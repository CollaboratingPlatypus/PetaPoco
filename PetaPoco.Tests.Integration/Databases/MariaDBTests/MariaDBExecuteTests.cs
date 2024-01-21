using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBExecuteTests : ExecuteTests
    {
        public MariaDBExecuteTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
