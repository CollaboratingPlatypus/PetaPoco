#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeDatabaseTests : BaseDatabaseTests
    {
        public MssqlCeDatabaseTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
