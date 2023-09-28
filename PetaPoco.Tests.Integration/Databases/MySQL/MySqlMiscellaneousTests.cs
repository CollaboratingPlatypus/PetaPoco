using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlMiscellaneousTests : BaseDatabase
    {
        public MySqlMiscellaneousTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}
