using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessTriageTests : BaseTriageTests
    {
        public MsAccessTriageTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}
