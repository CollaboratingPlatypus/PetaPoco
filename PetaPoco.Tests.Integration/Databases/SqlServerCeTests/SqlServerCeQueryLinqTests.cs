using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeQueryLinqTests : BaseQueryLinqTests
    {
        public MssqlCeQueryLinqTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
