using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdQueryTests : BaseQueryTests
    {
        public FirebirdQueryTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}