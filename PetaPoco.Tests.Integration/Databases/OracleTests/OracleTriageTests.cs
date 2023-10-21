using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleTriageTests : TriageTests
    {
        public OracleTriageTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
