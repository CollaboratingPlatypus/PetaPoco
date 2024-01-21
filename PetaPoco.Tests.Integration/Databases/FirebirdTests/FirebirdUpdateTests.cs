using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdUpdateTests : UpdateTests
    {
        public FirebirdUpdateTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
