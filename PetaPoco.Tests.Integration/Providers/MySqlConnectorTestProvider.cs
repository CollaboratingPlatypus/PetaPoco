namespace PetaPoco.Tests.Integration.Providers
{
    public class MySqlConnectorTestProvider : TestProvider
    {
        protected override string ConnectionName => "MySqlConnector";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}
