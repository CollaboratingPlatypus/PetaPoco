using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySql
{
    [Collection("MySql")]
    public class MySqlTriageTests : TriageTests
    {
        public MySqlTriageTests()
            : base(new MySqlTestProvider())
        {
        }
    }
}
