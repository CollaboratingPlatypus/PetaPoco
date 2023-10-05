using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataTriageTests : TriageTests
    {
        public SqlServerMSDataTriageTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }
    }
}
