using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlExecuteTests : BaseExecuteTests
    {
        public MySqlExecuteTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}