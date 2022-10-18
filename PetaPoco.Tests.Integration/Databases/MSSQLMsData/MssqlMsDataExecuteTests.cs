using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    [Trait("Category", "MssqlMsData")]
    public class MssqlMsDataExecuteTests : BaseExecuteTests
    {
        public MssqlMsDataExecuteTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
