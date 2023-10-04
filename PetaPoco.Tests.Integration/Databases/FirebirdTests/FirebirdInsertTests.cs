using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdInsertTests : BaseInsertTests
    {
        public FirebirdInsertTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}