using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeDeleteTests : BaseDeleteTests
    {
        public MssqlCeDeleteTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
