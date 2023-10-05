using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    [Collection("SqlServer")]
    public class SqlServerUpdateTests : UpdateTests
    {
        public SqlServerUpdateTests()
            : base(new SqlServerDbProviderFactory())
        {
        }
    }
}
