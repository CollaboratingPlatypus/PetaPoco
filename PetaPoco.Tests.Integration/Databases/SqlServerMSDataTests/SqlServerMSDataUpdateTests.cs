using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataUpdateTests : BaseUpdateTests
    {
        public MssqlMsDataUpdateTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
