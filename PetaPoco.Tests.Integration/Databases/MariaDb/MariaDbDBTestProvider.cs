namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "MariaDb";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDbBuildDatabase.sql";
    }
}