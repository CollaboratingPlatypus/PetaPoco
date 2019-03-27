using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    [Collection("MariaDb")]
    public class MariaDbInsertTests : BaseInsertTests
    {
        public MariaDbInsertTests()
            : base(new MariaDbDBTestProvider())
        {
        }
    }
}