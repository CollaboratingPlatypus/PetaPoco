namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => LoadFromConnectionName("Postgres");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}