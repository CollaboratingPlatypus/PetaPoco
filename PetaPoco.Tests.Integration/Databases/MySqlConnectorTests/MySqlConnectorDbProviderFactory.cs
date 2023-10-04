namespace PetaPoco.Tests.Integration.Databases.MySqlConnector
{
    public class MySqlConnectorDBTestProvider : BaseDbProviderFactory
    {
        protected override string ConnectionName => "mysqlconnector";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}