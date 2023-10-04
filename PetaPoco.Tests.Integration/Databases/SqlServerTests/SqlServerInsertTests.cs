using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlInsertTests : BaseInsertTests
    {
        public MssqlInsertTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}