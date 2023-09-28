using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlDatabaseTests : BaseDatabaseTests
    {
        public MssqlDatabaseTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
