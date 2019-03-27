using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlUpdateTests : BaseUpdateTests
    {
        public MySqlUpdateTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}