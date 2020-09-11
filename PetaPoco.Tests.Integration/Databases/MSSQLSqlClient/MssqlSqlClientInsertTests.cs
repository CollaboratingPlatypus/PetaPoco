using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientInsertTests : BaseInsertTests
    {
        public MssqlSqlClientInsertTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}