#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeExecuteTests : BaseExecuteTests
    {
        public MssqlCeExecuteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
