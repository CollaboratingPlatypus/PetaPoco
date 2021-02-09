using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataExecuteTests : BaseExecuteTests
    {
        public MssqlMsDataExecuteTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}