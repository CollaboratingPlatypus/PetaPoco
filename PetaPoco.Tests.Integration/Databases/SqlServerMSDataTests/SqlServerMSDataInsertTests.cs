using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataInsertTests : InsertTests
    {
        public SqlServerMSDataInsertTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
