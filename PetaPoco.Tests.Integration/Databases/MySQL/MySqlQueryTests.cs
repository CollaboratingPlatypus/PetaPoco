using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlQueryTests : BaseQueryTests
    {
        public MySqlQueryTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}