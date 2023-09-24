using Newtonsoft.Json.Linq;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models.Postgres
{
    [ExplicitColumns]
    [TableName("BugInvestigation_7K2TX4VR")]
    [PrimaryKey("Id")]
    public class JsonTypesPoco
    {
        [Column]
        public int Id { get; set; }

        [Column(InsertTemplate = "CAST({0}{1} AS json)", UpdateTemplate = "{0} = CAST({1}{2} AS json)")]
        public string Json1 { get; set; }

        [Column(InsertTemplate = "CAST({0}{1} AS json)", UpdateTemplate = "{0} = CAST({1}{2} AS json)")]
        public string Json2 { get; set; }

        public JsonTypesPoco()
        {
            Json1 = "{ \"firstName\":\"John\", \"lastName\":\"Doe\"}";
            Json2 = "{ \"firstName\":\"John\", \"lastName\":\"Doe\"}";
        }

        public void ShouldBe(JsonTypesPoco other)
        {
            Id.ShouldBe(other.Id);
            dynamic j1 = JObject.Parse(Json1);
            dynamic jo1 = JObject.Parse(other.Json1);
            dynamic j2 = JObject.Parse(Json2);
            dynamic jo2 = JObject.Parse(other.Json2);

            ((string)j1.firstName).ShouldBe((string)jo1.firstName);
            ((string)j1.lastName).ShouldBe((string)jo1.lastName);
            ((string)j2.firstName).ShouldBe((string)jo2.firstName);
            ((string)j2.lastName).ShouldBe((string)jo2.lastName);
        }

        public void ShouldNotBe(JsonTypesPoco other, bool sameId)
        {
            if (sameId)
            {
                Id.ShouldBe(other.Id);
            }
            else
            {
                Id.ShouldNotBe(other.Id);
            }

            dynamic j1 = JObject.Parse(Json1);
            dynamic jo1 = JObject.Parse(other.Json1);
            dynamic j2 = JObject.Parse(Json2);
            dynamic jo2 = JObject.Parse(other.Json2);

            ((string)j1.firstName).ShouldNotBe((string)jo1.firstName);
            ((string)j1.lastName).ShouldNotBe((string)jo1.lastName);
            ((string)j2.firstName).ShouldNotBe((string)jo2.firstName);
            ((string)j2.lastName).ShouldNotBe((string)jo2.lastName);
        }
    }
}
