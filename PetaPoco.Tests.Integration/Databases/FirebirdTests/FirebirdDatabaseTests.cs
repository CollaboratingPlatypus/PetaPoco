using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdDatabaseTests : BaseDatabaseTests
    {
        public FirebirdDatabaseTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}
