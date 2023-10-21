using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleDeleteTests : DeleteTests
    {
        public OracleDeleteTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
