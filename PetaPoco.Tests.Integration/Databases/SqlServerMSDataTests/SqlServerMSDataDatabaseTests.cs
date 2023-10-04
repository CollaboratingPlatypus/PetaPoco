using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("SqlServerMSData")]
    public class MssqlMsDataDatabaseTests : BaseDatabaseTests
    {
        public MssqlMsDataDatabaseTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
