using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlInsertTests : BaseInsertTests
    {
        public MySqlInsertTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}