using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataInsertTests : BaseInsertTests
    {
        public MssqlMsDataInsertTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
