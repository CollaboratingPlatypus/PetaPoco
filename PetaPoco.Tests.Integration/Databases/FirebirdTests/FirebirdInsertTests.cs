using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdInsertTests : InsertTests
    {
        public FirebirdInsertTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
