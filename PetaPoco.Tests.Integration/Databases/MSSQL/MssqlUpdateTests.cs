using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("MssqlTests")]
    public class MssqlUpdateTests : BaseUpdateTests
    {
        public MssqlUpdateTests()
            : base(new MssqlDBTestProvider())
        {
        }
    }
}