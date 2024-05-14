using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleQueryLinqTests : QueryLinqTests
    {
        protected OracleQueryLinqTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleQueryLinqTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleQueryLinqTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }
        }
    }
}
