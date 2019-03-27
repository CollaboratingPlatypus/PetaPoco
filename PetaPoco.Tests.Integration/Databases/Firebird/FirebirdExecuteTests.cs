using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdExecuteTests : BaseExecuteTests
    {
        public FirebirdExecuteTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}