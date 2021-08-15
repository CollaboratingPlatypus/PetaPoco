using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    [Trait("Category", "Mssql")]
    public class MssqlDatabaseTests : BaseDatabaseTests
    {
        public MssqlDatabaseTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}