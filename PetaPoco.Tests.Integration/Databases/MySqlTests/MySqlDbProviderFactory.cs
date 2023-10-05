namespace PetaPoco.Tests.Integration.Databases.MySql
{
    public class MySqlDbProviderFactory : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MySql";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}
