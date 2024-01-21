namespace PetaPoco.Tests.Integration.Providers
{
    public class MySqlTestProvider : TestProvider
    {
        protected override string ConnectionName => "MySql";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}
