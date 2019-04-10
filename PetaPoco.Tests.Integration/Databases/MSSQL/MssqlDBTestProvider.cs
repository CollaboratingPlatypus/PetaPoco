using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    public class MssqlDBTestProvider : DBTestProvider
    {
        private string _connectionName = "mssql";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLBuildDatabase.sql";

        public override IDatabase Execute()
        {
            EnsureDatabaseExists();
            return base.Execute();
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(s => { base.ExecuteBuildScript(database, s); });
        }

        private void EnsureDatabaseExists()
        {
            _connectionName = "mssql_builder";
            Database.Execute("IF(db_id(N'PetaPoco') IS NULL) BEGIN CREATE DATABASE[PetaPoco] END");
            _connectionName = "mssql";
        }
    }
}