using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeDatabaseTests : BaseDatabaseTests
    {
        public MssqlCeDatabaseTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
