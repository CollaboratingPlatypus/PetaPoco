using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessExecuteTests : BaseExecuteTests
    {
        public MsAccessExecuteTests()
            : base(new MsAccessDBTestProvider())
        {
        }
    }
}