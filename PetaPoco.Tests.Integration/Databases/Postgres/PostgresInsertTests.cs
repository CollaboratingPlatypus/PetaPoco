// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using PetaPoco.Tests.Integration.Models.Postgres;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("PostgresTests")]
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