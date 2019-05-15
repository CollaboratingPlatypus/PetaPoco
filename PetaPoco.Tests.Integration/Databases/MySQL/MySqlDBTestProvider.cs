namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    public class MySqlDBTestProvider : DBTestProvider
    {
        private string _connectionName = "MySQL";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        public override string ProviderName => GetProviderName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}