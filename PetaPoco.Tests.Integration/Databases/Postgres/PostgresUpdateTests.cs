// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using PetaPoco.Tests.Integration.Models.Postgres;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("PostgresTests")]
    public class PostgresUpdateTests : BaseUpdateTests
    {
        public PostgresUpdateTests()
            : base(new PostgresDBTestProvider())
        {
        }

        [Fact]
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