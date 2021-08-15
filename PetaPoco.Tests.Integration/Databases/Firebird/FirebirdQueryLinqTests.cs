using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdQueryLinqTests : BaseQueryLinqTests
    {
        public FirebirdQueryLinqTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}