namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : BaseDbProviderFactory
    {
        protected override string ConnectionName => "Postgres";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}