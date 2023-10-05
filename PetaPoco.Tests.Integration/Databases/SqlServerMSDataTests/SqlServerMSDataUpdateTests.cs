using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataUpdateTests : UpdateTests
    {
        public SqlServerMSDataUpdateTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
