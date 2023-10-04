using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlExecuteTests : BaseExecuteTests
    {
        public MssqlExecuteTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
