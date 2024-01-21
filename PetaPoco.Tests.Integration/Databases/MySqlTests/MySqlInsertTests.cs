using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlInsertTests : InsertTests
    {
        public MySqlInsertTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
