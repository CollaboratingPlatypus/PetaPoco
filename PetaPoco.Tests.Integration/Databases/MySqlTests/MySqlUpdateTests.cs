using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlUpdateTests : UpdateTests
    {
        public MySqlUpdateTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
