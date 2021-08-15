using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdDatabaseTests : BaseDatabaseTests
    {
        public FirebirdDatabaseTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}