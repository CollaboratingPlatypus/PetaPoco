using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleExecuteTests : ExecuteTests
    {
        public OracleExecuteTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
