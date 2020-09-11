using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientUpdateTests : BaseUpdateTests
    {
        public MssqlSqlClientUpdateTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}