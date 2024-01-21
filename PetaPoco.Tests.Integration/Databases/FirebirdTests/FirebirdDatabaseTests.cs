using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdDatabaseTests : DatabaseTests
    {
        public FirebirdDatabaseTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
