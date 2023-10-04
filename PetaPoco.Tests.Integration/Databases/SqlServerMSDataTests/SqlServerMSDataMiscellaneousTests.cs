using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlMsDataMiscellaneousTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
