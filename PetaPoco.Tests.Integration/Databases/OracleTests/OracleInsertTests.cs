using PetaPoco.Tests.Integration.Models.Postgres;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleInsertTests : InsertTests
    {
        public OracleInsertTests()
            : base(new OracleTestProvider())
        {
        }
    }
}
