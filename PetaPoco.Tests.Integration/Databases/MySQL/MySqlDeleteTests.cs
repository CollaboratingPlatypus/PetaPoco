using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    [Trait("Category", "Mysql")]
    public class MySqlDeleteTests : BaseDeleteTests
    {
        public MySqlDeleteTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}