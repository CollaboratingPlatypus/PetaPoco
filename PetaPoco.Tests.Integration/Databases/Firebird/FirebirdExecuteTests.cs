using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    [Trait("Category", "Firebird")]
    public class FirebirdExecuteTests : BaseExecuteTests
    {
        public FirebirdExecuteTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}