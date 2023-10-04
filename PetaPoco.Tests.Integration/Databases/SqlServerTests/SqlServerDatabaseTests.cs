using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlDatabaseTests : BaseDatabaseTests
    {
        public MssqlDatabaseTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
