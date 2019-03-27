using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlInsertTests : BaseInsertTests
    {
        public MySqlInsertTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}