using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlExecuteTests : BaseExecuteTests
    {
        public MySqlExecuteTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}