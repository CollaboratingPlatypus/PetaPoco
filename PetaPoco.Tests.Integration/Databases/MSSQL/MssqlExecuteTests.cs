using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlExecuteTests : BaseExecuteTests
    {
        public MssqlExecuteTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}