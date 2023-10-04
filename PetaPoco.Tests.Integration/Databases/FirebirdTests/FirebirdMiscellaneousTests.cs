using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdMiscellaneousTests : BaseMiscellaneousTests
    {
        public FirebirdMiscellaneousTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}
