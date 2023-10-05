using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlExecuteTests : ExecuteTests
    {
        public MySqlExecuteTests()
            : base(new MySqlDbProviderFactory())
        {
        }
    }
}
