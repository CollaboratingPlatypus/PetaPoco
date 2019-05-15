namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : DBTestProvider
    {
        private string _connectionName = "Postgres";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        public override string ProviderName => GetProviderName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}