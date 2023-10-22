using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using PetaPoco.Tests.Integration.Models;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    [Collection("Oracle")]
    public class OracleStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(OracleParameter);

        public OracleStoredProcTests()
            : base(new OracleTestProvider())
        {
        }

        private IDataParameter GetOutputParameter() => new OracleParameter("p_out_cursor", OracleDbType.RefCursor, ParameterDirection.Output);

        [Fact]
        public override void QueryProc_NoParam_ShouldReturnAll()
        {
            var results = DB.QueryProc<Person>("SelectPeople", GetOutputParameter()).ToArray();
            results.Length.ShouldBe(6);
        }

        [Fact]
        public override void QueryProc_WithParam_ShouldReturnSome()
        {
            var results = DB.QueryProc<Person>("SelectPeopleWithParam", new { age = 20 }, GetOutputParameter()).ToArray();
            results.Length.ShouldBe(3);
        }

        [Fact]
        public override void QueryProc_WithDbParam_ShouldReturnSome()
        {
            var results = DB.QueryProc<Person>("SelectPeopleWithParam", GetDataParameter(), GetOutputParameter()).ToArray();
            results.Length.ShouldBe(3);
        }

        [Fact]
        public override void FetchProc_NoParam_ShouldReturnAll()
        {
            var results = DB.FetchProc<Person>("SelectPeople", GetOutputParameter());
            results.Count.ShouldBe(6);
        }

        [Fact]
        public override void FetchProc_WithParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", new { age = 20 }, GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override void FetchProc_WithDbParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", GetDataParameter(), GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override void ScalarProc_NoParam_ShouldReturnAll()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeople", GetOutputParameter());
            count.ShouldBe(6);
        }

        [Fact]
        public override void ScalarProc_WithParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", new { age = 20 }, GetOutputParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public override void ScalarProc_WithDbParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", GetDataParameter(), GetOutputParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public override void NonQueryProc_NoParam_ShouldUpdateAll()
        {
            DB.ExecuteNonQueryProc("UpdatePeople");
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(6);
        }

        [Fact]
        public override void NonQueryProc_WithParam_ShouldUpdateSome()
        {
            DB.ExecuteNonQueryProc("UpdatePeopleWithParam", new { age = 20 });
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public override void NonQueryProc_WithDbParam_ShouldUpdateSome()
        {
            DB.ExecuteNonQueryProc("UpdatePeopleWithParam", GetDataParameter());
            DB.Query<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("FullName")}='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public override async Task QueryProcAsync_NoParam_ShouldReturnAll()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeople", GetOutputParameter());
            results.Count.ShouldBe(6);
        }

        [Fact]
        public override async Task QueryProcAsync_WithParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeopleWithParam", new { age = 20 }, GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task QueryProcAsync_WithDbParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            await DB.QueryProcAsync<Person>(p => results.Add(p), "SelectPeopleWithParam", GetDataParameter(), GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task QueryProcAsyncReader_NoParam_ShouldReturnAll()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeople", GetOutputParameter()))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(6);
        }

        [Fact]
        public override async Task QueryProcAsyncReader_WithParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeopleWithParam", new { age = 20 }, GetOutputParameter()))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task QueryProcAsyncReader_WithDbParam_ShouldReturnSome()
        {
            var results = new List<Person>();
            using (var reader = await DB.QueryProcAsync<Person>("SelectPeopleWithParam", GetDataParameter(), GetOutputParameter()))
            {
                while (await reader.ReadAsync())
                    results.Add(reader.Poco);
            }
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task FetchProcAsync_NoParam_ShouldReturnAll()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeople", GetOutputParameter());
            results.Count.ShouldBe(6);
        }

        [Fact]
        public override async Task FetchProcAsync_WithParam_ShouldReturnSome()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeopleWithParam", new { age = 20 }, GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task FetchProcAsync_WithDbParam_ShouldReturnSome()
        {
            var results = await DB.FetchProcAsync<Person>("SelectPeopleWithParam", GetDataParameter(), GetOutputParameter());
            results.Count.ShouldBe(3);
        }

        [Fact]
        public override async Task ScalarProcAsync_NoParam_ShouldReturnAll()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeople", GetOutputParameter());
            count.ShouldBe(6);
        }

        [Fact]
        public override async Task ScalarProcAsync_WithParam_ShouldReturnSome()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeopleWithParam", new { age = 20 }, GetOutputParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public override async Task ScalarProcAsync_WithDbParam_ShouldReturnSome()
        {
            var count = await DB.ExecuteScalarProcAsync<int>("CountPeopleWithParam", GetDataParameter(), GetOutputParameter());
            count.ShouldBe(3);
        }
    }
}
