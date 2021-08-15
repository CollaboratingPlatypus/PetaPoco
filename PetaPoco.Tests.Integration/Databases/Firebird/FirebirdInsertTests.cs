using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdInsertTests : BaseInsertTests
    {
        public FirebirdInsertTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}