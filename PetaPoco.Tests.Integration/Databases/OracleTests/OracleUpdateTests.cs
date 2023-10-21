using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleUpdateTests : UpdateTests
    {
        public OracleUpdateTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
