#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeUpdateTests : BaseUpdateTests
    {
        public MssqlCeUpdateTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
