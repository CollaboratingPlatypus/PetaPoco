#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeTriageTests : BaseTriageTests
    {
        public MssqlCeTriageTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
