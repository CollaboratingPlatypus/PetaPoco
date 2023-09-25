using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlMsDataMiscellaneousTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
