using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessMiscellaneousTests : MiscellaneousTests
    {
        public MSAccessMiscellaneousTests()
            : base(new MSAccessDbProviderFactory())
        {
        }
    }
}
