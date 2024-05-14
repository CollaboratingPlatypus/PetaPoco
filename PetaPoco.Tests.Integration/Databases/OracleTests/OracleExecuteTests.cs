using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleExecuteTests : ExecuteTests
    {
        protected OracleExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleExecuteTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleExecuteTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }
        }
    }
}
