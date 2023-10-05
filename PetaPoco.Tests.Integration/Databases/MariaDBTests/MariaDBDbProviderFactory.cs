namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    public class MariaDBDbProviderFactory : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MariaDB";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDBBuildDatabase.sql";
    }
}
