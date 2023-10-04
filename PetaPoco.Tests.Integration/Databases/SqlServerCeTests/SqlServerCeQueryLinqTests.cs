#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeQueryLinqTests : BaseQueryLinqTests
    {
        public MssqlCeQueryLinqTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
