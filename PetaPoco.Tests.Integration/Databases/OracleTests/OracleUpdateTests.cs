using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleUpdateTests : UpdateTests
    {
        protected OracleUpdateTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleUpdateTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleUpdateTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }
        }
    }
}
