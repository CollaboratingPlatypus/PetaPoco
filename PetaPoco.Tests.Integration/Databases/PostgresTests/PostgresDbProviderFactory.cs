namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDbProviderFactory : BaseDbProviderFactory
    {
        protected override string ConnectionName => "Postgres";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}
