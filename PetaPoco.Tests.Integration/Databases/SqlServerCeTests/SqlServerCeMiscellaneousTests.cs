#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlCeMiscellaneousTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
