using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdMiscellaneousTests : MiscellaneousTests
    {
        public FirebirdMiscellaneousTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
