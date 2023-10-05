using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlDeleteTests : DeleteTests
    {
        public MySqlDeleteTests()
            : base(new MySqlDbProviderFactory())
        {
        }
    }
}
