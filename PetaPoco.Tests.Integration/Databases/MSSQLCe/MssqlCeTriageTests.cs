using Xunit;

#if MSSQLCE_TESTS_ENABLED
namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("MssqlCe")]
    public class MssqlCeTriageTests : BaseTriageTests
    {
        public MssqlCeTriageTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
