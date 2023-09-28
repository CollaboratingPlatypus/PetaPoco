using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdTriageTests : BaseTriageTests
    {
        public FirebirdTriageTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}
