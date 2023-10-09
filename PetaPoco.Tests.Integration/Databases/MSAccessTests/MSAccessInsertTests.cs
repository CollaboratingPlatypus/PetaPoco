using Xunit;
using PetaPoco.Tests.Integration.Providers;


namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessInsertTests : InsertTests
    {
        public MSAccessInsertTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
