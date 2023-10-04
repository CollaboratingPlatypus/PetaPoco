using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataExecuteTests : BaseExecuteTests
    {
        public MssqlMsDataExecuteTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
