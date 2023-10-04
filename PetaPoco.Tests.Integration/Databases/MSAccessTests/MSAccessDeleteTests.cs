using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessDeleteTests : BaseDeleteTests
    {
        public MsAccessDeleteTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}