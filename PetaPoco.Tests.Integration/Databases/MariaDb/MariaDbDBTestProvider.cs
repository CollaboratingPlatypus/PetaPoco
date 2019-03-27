namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => LoadFromConnectionName("MariaDb");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDbBuildDatabase.sql";
    }
}