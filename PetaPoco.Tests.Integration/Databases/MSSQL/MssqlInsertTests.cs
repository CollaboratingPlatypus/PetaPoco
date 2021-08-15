using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    [Trait("Category", "Mssql")]
    public class MssqlInsertTests : BaseInsertTests
    {
        public MssqlInsertTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}