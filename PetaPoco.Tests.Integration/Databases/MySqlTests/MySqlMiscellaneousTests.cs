using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlMiscellaneousTests : MiscellaneousTests
    {
        public MySqlMiscellaneousTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
