using System;
using System.Linq;

/* 
 * Converted build scripts from SqlServerBuildDatabase.sql using MSSQLTips.com for datatype conversions
 * Link: https://www.mssqltips.com/sqlservertip/2944/comparing-sql-server-and-oracle-datatypes
 */

namespace PetaPoco.Tests.Integration.Providers
{
    public class OracleTestProvider : TestProvider
    {
        private static readonly string[] _splitSemiColon = new[] { ";" };
        private static readonly string[] _splitNewLine = new[] { Environment.NewLine };
        private static readonly string[] _splitSlash = new[] { Environment.NewLine + "/" };
        private static readonly string[] _resources = new[]
        {
            "PetaPoco.Tests.Integration.Scripts.OracleSetupDatabase.sql",
            "PetaPoco.Tests.Integration.Scripts.OracleBuildDatabase.sql"
        };
        private static ExecutionPhase _phase = ExecutionPhase.Setup;

        private string _connectionName = "Oracle";
        protected override string ConnectionName => _connectionName;

        protected override string ScriptResourceName => _resources[(int)_phase];

        public override IDatabase Execute()
        {
            EnsureDatabaseSetup();
            return base.Execute();
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            //The script file can contain multiple script blocks, separated by a line containing a single forward slash ("\r\n/").
            //Script blocks end with "END;" and can execute as a whole.
            //Statements are separated by a semi colon and have to be executed separately.
            //This "one statement at a time" limitation is due to the database provider.

            script.Split(_splitSlash, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => StripLineComments(s).Trim()).ToList()
                .ForEach(s =>
                {
                    if (string.IsNullOrEmpty(s)) return;

                    if (s.EndsWith("END;", StringComparison.OrdinalIgnoreCase))
                    {
                        base.ExecuteBuildScript(database, s);
                        return;
                    }

                    s.Split(_splitSemiColon, StringSplitOptions.RemoveEmptyEntries).ToList()
                    .ForEach(x => base.ExecuteBuildScript(database, x));
                });
        }

        private void EnsureDatabaseSetup()
        {
            //No need to run database setup scripts for every test
            if (_phase != ExecutionPhase.Setup) return;

            var previousName = _connectionName;
            _connectionName = "Oracle_Builder";

            _ = base.Execute();

            _connectionName = previousName;
            _phase = ExecutionPhase.Build;
        }

        private string StripLineComments(string script)
        {
            var parts = script.Split(_splitNewLine, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !s.Trim().StartsWith("--"));
            return string.Join(_splitNewLine[0], parts);
        }
    }
}
