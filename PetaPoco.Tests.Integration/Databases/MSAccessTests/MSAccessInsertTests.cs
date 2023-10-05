using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessInsertTests : InsertTests
    {
        public MSAccessInsertTests()
            : base(new MSAccessDbProviderFactory())
        {
        }
    }
}
