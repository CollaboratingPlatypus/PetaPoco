using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    public class MssqlMsDataDBTestProvider : BaseDbProviderFactory
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
            Database.Execute("IF(db_id(N'PetaPocoMsData') IS NULL) BEGIN CREATE DATABASE [PetaPocoMsData] END");
            _connectionName = "SqlServerMSData";
        }
    }
}
