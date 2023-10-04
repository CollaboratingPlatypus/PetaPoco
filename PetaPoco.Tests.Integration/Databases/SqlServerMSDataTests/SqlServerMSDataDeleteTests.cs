using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataDeleteTests : BaseDeleteTests
    {
        public MssqlMsDataDeleteTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
