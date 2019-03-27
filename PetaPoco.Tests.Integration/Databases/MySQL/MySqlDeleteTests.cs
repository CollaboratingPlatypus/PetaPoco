using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlDeleteTests : BaseDeleteTests
    {
        public MySqlDeleteTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}