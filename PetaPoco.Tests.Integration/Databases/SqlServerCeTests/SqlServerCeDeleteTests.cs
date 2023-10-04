#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeDeleteTests : BaseDeleteTests
    {
        public MssqlCeDeleteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
