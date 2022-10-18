using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    public class MssqlMsDataDBTestProvider : DBTestProvider
    {
        private string _connectionName = "mssqlmsdata";
        protected override string ConnectionName => _connectionName;

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLBuildDatabase.sql";

        public override IDatabase Execute()
        {
            EnsureDatabaseExists();
            return base.Execute();
        }

        protected override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(s => { base.ExecuteBuildScript(database, s); });
        }

        private void EnsureDatabaseExists()
        {
            _connectionName = "mssqlmsdata_builder";
            Database.Execute("IF(db_id(N'PetaPocoMsData') IS NULL) BEGIN CREATE DATABASE [PetaPocoMsData] END");
            _connectionName = "mssqlmsdata";
        }
    }
}