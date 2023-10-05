using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdDeleteTests : DeleteTests
    {
        public FirebirdDeleteTests()
            : base(new FirebirdDbProviderFactory())
        {
        }
    }
}
