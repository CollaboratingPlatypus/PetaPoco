namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    public class MySqlDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => LoadFromConnectionName("MySQL");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}