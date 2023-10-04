using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessDatabaseTests : BaseDatabaseTests
    {
        public MsAccessDatabaseTests()
            : base(new MsAccessDBTestProvider())
        {
        }

        [Fact(Skip = "Isolation Levels not supported by provider.")]
        public override void BeginTransaction_WhenIsolationLevelIsSet_ShouldBeOfIsolationLevel() { }
    }
}
