using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlUpdateTests : BaseUpdateTests
    {
        public MySqlUpdateTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}