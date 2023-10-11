using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessTriageTests : TriageTests
    {
        public MSAccessTriageTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
