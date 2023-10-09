using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlDatabaseTests : DatabaseTests
    {
        public MySqlDatabaseTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
