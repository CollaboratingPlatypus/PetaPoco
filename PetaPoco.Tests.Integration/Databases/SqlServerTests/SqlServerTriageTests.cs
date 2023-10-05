using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerTriageTests : TriageTests
    {
        public SqlServerTriageTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
