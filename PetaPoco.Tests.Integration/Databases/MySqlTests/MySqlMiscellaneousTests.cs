using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlMiscellaneousTests : BaseMiscellaneousTests
    {
        public MySqlMiscellaneousTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}
