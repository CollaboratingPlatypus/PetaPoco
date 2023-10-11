using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDB
{
    [Collection("MariaDB")]
    public class MariaDBInsertTests : InsertTests
    {
        public MariaDBInsertTests()
            : base(new MariaDBTestProvider())
        {
        }
    }
}
