namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    public class MySqlDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "MySQL";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}