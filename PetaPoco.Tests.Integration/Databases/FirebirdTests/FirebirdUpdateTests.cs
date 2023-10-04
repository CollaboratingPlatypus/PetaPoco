using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdUpdateTests : BaseUpdateTests
    {
        public FirebirdUpdateTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}