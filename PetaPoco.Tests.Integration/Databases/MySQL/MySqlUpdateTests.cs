using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySqlTests")]
    public class MySqlUpdateTests : BaseUpdateTests
    {
        public MySqlUpdateTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}