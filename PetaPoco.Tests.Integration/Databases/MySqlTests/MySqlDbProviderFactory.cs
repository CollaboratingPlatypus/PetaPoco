namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    public class MySqlDBTestProvider : BaseDbProviderFactory
    {
        protected override string ConnectionName => "MySQL";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}