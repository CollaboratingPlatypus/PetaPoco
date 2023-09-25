using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySql")]
    public class MySqlTriageTests : BaseTriageTests
    {
        public MySqlTriageTests()
            : base(new MySqlDBTestProvider())
        {
        }
    }
}
