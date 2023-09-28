using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlMiscellaneousTests : BaseMiscellaneousTests
    {
        public MssqlMiscellaneousTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
