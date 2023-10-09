using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessExecuteTests : ExecuteTests
    {
        public MSAccessExecuteTests()
            : base(new MSAccessTestProvider())
        {
        }
    }
}
