using PetaPoco.Tests.Integration.Models.Postgres;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresInsertTests : BaseInsertTests
    {
        public PostgresInsertTests()
            : base(new PostgresDBTestProvider())
        {
        }

        [Fact]
        public void Insert_GivenPocoWithJsonTypes_ShouldBeValid()
        {
            var poco = new JsonTypesPoco();

            var id = DB.Insert(poco);
            var pocoOther = DB.Single<JsonTypesPoco>(poco.Id);

            poco.Id.ShouldBe(id);
            pocoOther.ShouldNotBeNull();
            pocoOther.ShouldBe(poco);
        }
    }
}