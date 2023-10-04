using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeInsertTests : BaseInsertTests
    {
        public MssqlCeInsertTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
