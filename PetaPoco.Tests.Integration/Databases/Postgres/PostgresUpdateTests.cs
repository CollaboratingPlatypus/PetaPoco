using PetaPoco.Tests.Integration.Models.Postgres;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresUpdateTests : BaseUpdateTests
    {
        public PostgresUpdateTests()
            : base(new PostgresDBTestProvider())
        {
        }

        // TODO: Issue still open since 2016: https://github.com/CollaboratingPlatypus/PetaPoco/issues/318
        [Fact]
        [Trait("Issue", "#318")]
        public void Update_GivenPocoWithJsonTypes_ShouldBeValid()
        {
            var poco = new JsonTypesPoco();

            var id = DB.Insert(poco);
            var pocoOther = DB.Single<JsonTypesPoco>(poco.Id);
            pocoOther.Json1 = "{\"firstName\":\"Peta\", \"lastName\":\"Poco\"}";
            pocoOther.Json2 = "{\"firstName\":\"Peta\", \"lastName\":\"Poco\"}";
            DB.Update(pocoOther);

            poco.Id.ShouldBe(id);
            pocoOther.ShouldNotBeNull();
            pocoOther.ShouldNotBe(poco, true);
        }
    }
}
