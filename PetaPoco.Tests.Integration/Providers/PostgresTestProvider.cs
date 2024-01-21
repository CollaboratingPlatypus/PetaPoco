namespace PetaPoco.Tests.Integration.Providers
{
    public class PostgresTestProvider : TestProvider
    {
        protected override string ConnectionName => "Postgres";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}
