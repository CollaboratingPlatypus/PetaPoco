using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlQueryLinqTests : QueryLinqTests
    {
        public MySqlQueryLinqTests()
            : base(new MySqlDbProviderFactory())
        {
        }
    }
}
