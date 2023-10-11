using PetaPoco.Tests.Integration.Models.Postgres;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresInsertTests : InsertTests
    {
        public PostgresInsertTests()
            : base(new PostgresTestProvider())
        {
        }

        // TODO: Issue still open since 2016: https://github.com/CollaboratingPlatypus/PetaPoco/issues/318
        [Fact]
        [Trait("Issue", "#318")]
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
