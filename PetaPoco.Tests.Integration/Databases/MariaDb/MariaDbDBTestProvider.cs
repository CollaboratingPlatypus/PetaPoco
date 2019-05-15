namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : DBTestProvider
    {
        private string _connectionName = "MariaDb";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDbBuildDatabase.sql";

        public override string ProviderName => GetProviderName(_connectionName);
    }
}