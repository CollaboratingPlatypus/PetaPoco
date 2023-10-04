using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlCeMiscellaneousTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
