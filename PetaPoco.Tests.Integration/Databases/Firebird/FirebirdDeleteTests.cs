using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdDeleteTests : BaseDeleteTests
    {
        public FirebirdDeleteTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}