using PetaPoco.Tests.Integration.Databases;
using Xunit;

namespace PetaPoco.Tests.Integration.x86.Databases.MSAccess
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