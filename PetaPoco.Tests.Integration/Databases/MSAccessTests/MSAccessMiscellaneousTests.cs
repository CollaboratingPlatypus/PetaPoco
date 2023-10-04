using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessMiscellaneousTests : BaseMiscellaneousTests
    {
        public MsAccessMiscellaneousTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}
