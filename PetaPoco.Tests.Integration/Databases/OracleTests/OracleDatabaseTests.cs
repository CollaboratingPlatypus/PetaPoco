using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleDatabaseTests : DatabaseTests
    {
        public OracleDatabaseTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
