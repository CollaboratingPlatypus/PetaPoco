namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MariaDb";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDbBuildDatabase.sql";
    }
}