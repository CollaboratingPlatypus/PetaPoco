using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    public class MsAccessDBTestProvider : DBTestProvider
    {
        private string _connectionName = "msaccess";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        public override string ProviderName => GetProviderName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSAccessBuildDatabase.sql";

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().ForEach(s =>
            {
                if (s.StartsWith("--"))
                    return;

                if (s.StartsWith("DROP"))
                {
                    try
                    {
                        base.ExecuteBuildScript(database, s);
                    }
                    catch
                    {
                    }

                    return;
                }

                base.ExecuteBuildScript(database, s);
            });
        }
    }
}