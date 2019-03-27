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

        public override void BeginTransaction_WhenIsolationLevelIsSet_ShouldBeOfIsolationLevel()
        {
            // Not supported by provider.
        }
    }
}