using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlMiscellaneousTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
