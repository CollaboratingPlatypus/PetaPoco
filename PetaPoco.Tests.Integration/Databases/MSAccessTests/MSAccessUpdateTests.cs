using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessUpdateTests : BaseUpdateTests
    {
        public MsAccessUpdateTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}