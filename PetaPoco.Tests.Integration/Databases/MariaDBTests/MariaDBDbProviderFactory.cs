namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MariaDB";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDBBuildDatabase.sql";
    }
}
