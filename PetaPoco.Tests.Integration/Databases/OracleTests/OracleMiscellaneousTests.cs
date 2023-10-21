using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleMiscellaneousTests : MiscellaneousTests
    {
        public OracleMiscellaneousTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
