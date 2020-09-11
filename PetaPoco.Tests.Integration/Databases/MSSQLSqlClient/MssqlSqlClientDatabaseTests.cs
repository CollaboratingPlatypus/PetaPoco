using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientDatabaseTests : BaseDatabaseTests
    {
        public MssqlSqlClientDatabaseTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}