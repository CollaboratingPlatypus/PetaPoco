using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdTriageTests : TriageTests
    {
        public FirebirdTriageTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
