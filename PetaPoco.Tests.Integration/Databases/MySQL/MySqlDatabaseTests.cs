using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlDatabaseTests : BaseDatabaseTests
    {
        public MySqlDatabaseTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}