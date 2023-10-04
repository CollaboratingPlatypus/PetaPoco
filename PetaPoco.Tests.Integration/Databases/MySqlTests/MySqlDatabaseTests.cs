using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlDatabaseTests : BaseDatabaseTests
    {
        public MySqlDatabaseTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}
