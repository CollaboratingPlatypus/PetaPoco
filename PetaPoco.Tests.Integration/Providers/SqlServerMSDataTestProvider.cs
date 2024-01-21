using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Providers
{
    public class SqlServerMSDataTestProvider : TestProvider
    {
        private string _connectionName = "SqlServerMSData";
        protected override string ConnectionName => _connectionName;

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqlServerBuildDatabase.sql";

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
            _connectionName = "SqlServerMSData_Builder";
            Database.Execute("IF(db_id(N'PetaPocoMSData') IS NULL) BEGIN CREATE DATABASE [PetaPocoMSData] END");
            _connectionName = "SqlServerMSData";
        }
    }
}
