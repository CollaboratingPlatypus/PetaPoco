using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlQueryLinqTests : BaseQueryLinqTests
    {
        public MySqlQueryLinqTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}