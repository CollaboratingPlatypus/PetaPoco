using PetaPoco.Tests.Integration.Models.Postgres;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleInsertTests : InsertTests
    {
        protected OracleInsertTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleInsertTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleInsertTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }
        }
    }
}
