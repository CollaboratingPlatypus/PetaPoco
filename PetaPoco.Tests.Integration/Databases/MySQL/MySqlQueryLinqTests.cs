using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlQueryLinqTests : BaseQueryLinqTests
    {
        public MySqlQueryLinqTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}