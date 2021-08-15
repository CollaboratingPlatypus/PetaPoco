using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    [Trait("Category", "Mssql")]
    public class MssqlUpdateTests : BaseUpdateTests
    {
        public MssqlUpdateTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}