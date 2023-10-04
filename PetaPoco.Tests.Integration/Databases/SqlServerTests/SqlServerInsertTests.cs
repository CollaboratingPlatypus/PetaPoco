using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("SqlServer")]
    public class MssqlInsertTests : BaseInsertTests
    {
        public MssqlInsertTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}
