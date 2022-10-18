namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    public class MySqlConnectorDBTestProvider : DBTestProvider
    {
        private string _connectionName = "mysqlconnector";
        protected override string ConnectionName => _connectionName;

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";

        public override IDatabase Execute()
        {
            EnsureDatabaseExists();
            return base.Execute();
        }

        private void EnsureDatabaseExists()
        {
            _connectionName = "mysql_builder";
            Database.Execute("CREATE DATABASE IF NOT EXISTS petapocoConnector");
            Database.Execute("CREATE USER IF NOT EXISTS 'petapocoConnector'@'%' IDENTIFIED BY 'petapoco'");
            Database.Execute("GRANT ALL PRIVILEGES ON petapocoConnector.* TO 'petapocoConnector'@'%'");
            _connectionName = "mysqlconnector";
        }
    }
}
