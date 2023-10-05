using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdExecuteTests : ExecuteTests
    {
        public FirebirdExecuteTests()
            : base(new FirebirdDbProviderFactory())
        {
        }
    }
}
