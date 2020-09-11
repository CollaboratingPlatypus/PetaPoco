using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLSqlClient
{
    [Collection("MssqlSqlClient")]
    public class MssqlSqlClientDeleteTests : BaseDeleteTests
    {
        public MssqlSqlClientDeleteTests()
            : base(new MssqlSqlClientDBTestProvider())
        {
        }
    }
}