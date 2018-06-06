using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccessTests")]
    public class MsAccessUpdateTests : BaseUpdateTests
    {
        public MsAccessUpdateTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}