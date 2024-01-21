using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessDatabaseTests : DatabaseTests
    {
        public MSAccessDatabaseTests()
            : base(new MSAccessTestProvider())
        {
        }

        [Fact(Skip = "Isolation Levels not supported by provider.")]
        public override void BeginTransaction_WhenIsolationLevelIsSet_ShouldBeOfIsolationLevel() { }
    }
}
