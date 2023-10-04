using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeExecuteTests : BaseExecuteTests
    {
        public MssqlCeExecuteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
