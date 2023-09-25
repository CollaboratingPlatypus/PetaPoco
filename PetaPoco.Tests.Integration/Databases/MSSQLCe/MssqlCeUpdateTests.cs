using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeUpdateTests : BaseUpdateTests
    {
        public MssqlCeUpdateTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
