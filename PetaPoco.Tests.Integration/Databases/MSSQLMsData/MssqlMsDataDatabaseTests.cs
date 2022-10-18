using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    [Trait("Category", "MssqlMsData")]
    public class MssqlMsDataDatabaseTests : BaseDatabaseTests
    {
        public MssqlMsDataDatabaseTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }
    }
}
