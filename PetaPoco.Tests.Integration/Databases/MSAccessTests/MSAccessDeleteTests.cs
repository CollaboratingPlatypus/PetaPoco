using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessDeleteTests : DeleteTests
    {
        public MSAccessDeleteTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
