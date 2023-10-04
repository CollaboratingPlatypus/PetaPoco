#if MSSQLCE_TESTS_ENABLED
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    [Collection("SqlServerCe")]
    public class MssqlCeInsertTests : BaseInsertTests
    {
        public MssqlCeInsertTests()
            : base(new MssqlCeDBTestProvider())
        {
        }
    }
}
#endif
