using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    public class MssqlSqlClientDBTestProvider : DBTestProvider
    {
        private string _connectionName = "mssqlsqlclient";
        protected override string ConnectionName => _connectionName;

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
            _connectionName = "mssqlsqlclient_builder";
            Database.Execute("IF(db_id(N'PetaPoco2') IS NULL) BEGIN CREATE DATABASE [PetaPoco2] END");
            _connectionName = "mssqlsqlclient";
        }
    }
}