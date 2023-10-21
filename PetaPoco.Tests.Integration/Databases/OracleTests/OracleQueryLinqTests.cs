using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleQueryLinqTests : QueryLinqTests
    {
        public OracleQueryLinqTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
