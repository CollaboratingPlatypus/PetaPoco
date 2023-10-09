using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessUpdateTests : UpdateTests
    {
        public MSAccessUpdateTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
