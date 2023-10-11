using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessQueryLinqTests : QueryLinqTests
    {
        public MSAccessQueryLinqTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
