namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    public class MySqlConnectorDbProviderFactory : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MySqlConnector";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}
