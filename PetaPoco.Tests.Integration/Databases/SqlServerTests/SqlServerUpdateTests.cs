using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlUpdateTests : BaseUpdateTests
    {
        public MssqlUpdateTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}