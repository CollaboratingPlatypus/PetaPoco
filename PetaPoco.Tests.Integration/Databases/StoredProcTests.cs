using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration
{
    public abstract class StoredProcTests : BaseDbContext
    {
        protected abstract Type DataParameterType { get; }

        protected StoredProcTests(TestProvider provider)
            : base(provider)
        {
            AddPeople(6);
        }

        #region Test Helpers

        protected void AddPeople(int peopleToAdd)
        {
            for (var i = 0; i < peopleToAdd; i++)
            {
                var p = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Peta" + i,
                    Age = 18 + i,
                    Dob = new DateTime(1980 - (18 + i), 1, 1, 1, 1, 1, DateTimeKind.Utc),
                };
                DB.Insert(p);
            }
        }

        protected IDataParameter GetDataParameter()
        {
            var param = Activator.CreateInstance(DataParameterType) as IDataParameter;
            param.ParameterName = "age";
            param.Value = 20;
            return param;
        }

        #endregion

        [Fact]
        public virtual void QueryProc_NoParam_ShouldReturnAll()
        {
            var results = DB.QueryProc<Person>("SelectPeople").ToArray();
            results.Length.ShouldBe(6);
        }

        [Fact]
        public virtual void QueryProc_WithParam_ShouldReturnSome()
        {
            var results = DB.QueryProc<Person>("SelectPeopleWithParam", new { age = 20 }).ToArray();
            results.Length.ShouldBe(3);
        }

        [Fact]
        public virtual void QueryProc_WithDbParam_ShouldReturnSome()
        {
            var results = DB.QueryProc<Person>("SelectPeopleWithParam", GetDataParameter()).ToArray();
            results.Length.ShouldBe(3);
        }

        [Fact]
        public virtual void FetchProc_NoParam_ShouldReturnAll()
        {
            var results = DB.FetchProc<Person>("SelectPeople");
            results.Count.ShouldBe(6);
        }

        [Fact]
        public virtual void FetchProc_WithParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", new { age = 20 });
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual void FetchProc_WithDbParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", GetDataParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual void ScalarProc_NoParam_ShouldReturnAll()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeople");
            count.ShouldBe(6);
        }

        [Fact]
        public virtual void ScalarProc_WithParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", new { age = 20 });
            count.ShouldBe(3);
        }

        [Fact]
        public virtual void ScalarProc_WithDbParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", GetDataParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public virtual void NonQueryProc_NoParam_ShouldUpdateAll()
        {
            DB.ExecuteNonQueryProc("UpdatePeople");
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(6);
        }

        [Fact]
        public virtual void NonQueryProc_WithParam_ShouldUpdateSome()
        {
            DB.ExecuteNonQueryProc("UpdatePeopleWithParam", new { age = 20 });
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public virtual void NonQueryProc_WithDbParam_ShouldUpdateSome()
        {
            DB.ExecuteNonQueryProc("UpdatePeopleWithParam", GetDataParameter());
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public virtual async Task QueryProcAsync_NoParam_ShouldReturnAll()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeople");
            results.Count.ShouldBe(6);
        }

        [Fact]
        public virtual async Task QueryProcAsync_WithParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeopleWithParam", new { age = 20 });
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task QueryProcAsync_WithDbParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeopleWithParam", GetDataParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task QueryProcAsyncReader_NoParam_ShouldReturnAll()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeople"))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(6);
        }

        [Fact]
        public virtual async Task QueryProcAsyncReader_WithParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeopleWithParam", new { age = 20 }))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task QueryProcAsyncReader_WithDbParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeopleWithParam", GetDataParameter()))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task FetchProcAsync_NoParam_ShouldReturnAll()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeople");
            results.Count.ShouldBe(6);
        }

        [Fact]
        public virtual async Task FetchProcAsync_WithParam_ShouldReturnSome()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeopleWithParam", new { age = 20 });
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task FetchProcAsync_WithDbParam_ShouldReturnSome()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeopleWithParam", GetDataParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task ScalarProcAsync_NoParam_ShouldReturnAll()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeople");
            count.ShouldBe(6);
        }

        [Fact]
        public virtual async Task ScalarProcAsync_WithParam_ShouldReturnSome()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeopleWithParam", new { age = 20 });
            count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task ScalarProcAsync_WithDbParam_ShouldReturnSome()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeopleWithParam", GetDataParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public virtual async Task NonQueryProcAsync_NoParam_ShouldUpdateAll()
        {
            await DB.ExecuteNonQueryProcAsync("UpdatePeople");
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(6);
        }

        [Fact]
        public virtual async Task NonQueryProcAsync_WithParam_ShouldUpdateSome()
        {
            await DB.ExecuteNonQueryProcAsync("UpdatePeopleWithParam", new { age = 20 });
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public virtual async Task NonQueryProcAsync_WithDbParam_ShouldUpdateSome()
        {
            await DB.ExecuteNonQueryProcAsync("UpdatePeopleWithParam", GetDataParameter());
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        [Trait("Issue", "#657")]
        public virtual async Task SchemaVariantProc_WithParam_ShouldReturnCorrectDynamicTypeCollection()
        {
            var expectedFirst = new Dictionary<string, object>
            {
                { "One", 1 },
                { "Two", 2 },
                { "Three", 3 }
            };
            var expectedSecond = new Dictionary<string, object>
            {
                { "Ten", 10 },
                { "Twenty", 20 },
                { "Thirty", 30 }
            };

            IDictionary<string, object> actualFirst = (await DB.FetchProcAsync<dynamic>("SchemaVariantProc", new { schemaIndex = 0 })).First();
            foreach (var item in actualFirst)
            {
                expectedFirst.ShouldContainKey(item.Key);
                expectedFirst[item.Key].ShouldBe(item.Value);
            }

            IDictionary<string, object> actualSecond = (await DB.FetchProcAsync<dynamic>("SchemaVariantProc", new { schemaIndex = 1 })).First();
            foreach (var item in actualSecond)
            {
                expectedSecond.ShouldContainKey(item.Key);
                expectedSecond[item.Key].ShouldBe(item.Value);
            }
        }
    }
}
