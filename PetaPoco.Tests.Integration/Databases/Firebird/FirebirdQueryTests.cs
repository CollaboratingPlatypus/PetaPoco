using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdQueryTests : BaseQueryTests
    {
        public FirebirdQueryTests()
            : base(new FirebirdDBTestProvider())
        {
        }
    }
}