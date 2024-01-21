namespace PetaPoco.Tests.Integration.Providers
{
    public class MariaDBTestProvider : TestProvider
    {
        protected override string ConnectionName => "MariaDB";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDBBuildDatabase.sql";
    }
}
