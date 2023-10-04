using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlUpdateTests : BaseUpdateTests
    {
        public MssqlUpdateTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
