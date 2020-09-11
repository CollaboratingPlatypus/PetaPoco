using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientExecuteTests : BaseExecuteTests
    {
        public MssqlSqlClientExecuteTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}