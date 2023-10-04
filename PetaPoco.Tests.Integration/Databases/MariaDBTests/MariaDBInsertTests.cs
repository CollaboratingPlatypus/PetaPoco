using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDB")]
    public class MariaDbInsertTests : BaseInsertTests
    {
        public MariaDbInsertTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}
