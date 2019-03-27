using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class CreateCommandTests : IDisposable
    {
        public readonly DbConnection _conn = new SqlConnection();
        public readonly Database _db = new Database("foo", "System.Data.SqlClient");

        public static IEnumerable<object[]> QueryParamData
            => new[]
            {
                new object[]
                {
                    "select * from foo where bar=@0",
                    new object[] { "baz" },
                    new (string, object)[] { ("@0", "baz") }
                },
                new object[]
                {
                    "select * from MyTable where bar=@0 and baz=@1",
                    new object[] { 3, 7 },
                    new (string, object)[] { ("@0", 3), ("@1", 7) }
                },
                new object[]
                {
                    "select * from foo",
                    new object[0],
                    new (string, object)[0]
                },
            };

        public static IEnumerable<object[]> ProcParamData
            => new[]
            {
                new object[]
                {
                    "AnonymousType",
                    new object[] { new { Foo = "Bar", Baz = 3 } },
                    new (string, object)[] { ("@Foo", "Bar"), ("@Baz", 3) }
                },
                new object[]
                {
                    "TwoAnonymousTypes",
                    new object[] { new { Foo = "Bar" }, new { Baz = 3 } },
                    new (string, object)[] { ("@Foo", "Bar"), ("@Baz", 3) }
                },
                new object[]
                {
                    "SqlParameter",
                    new object[] { new SqlParameter("@Foo", "Bar") },
                    new (string, object)[] { ("@Foo", "Bar") }
                },
                new object[]
                {
                    "NoArgs",
                    new object[0],
                    new (string, object)[0]
                },
                new object[]
                {
                    "Array",
                    new object[] { new object[] { new { Foo = "Bar" }, new { Baz = 3 } } },
                    new (string, object)[] { ("@Foo", "Bar"), ("@Baz", 3) }
                },
            };

        public void Dispose()
        {
            _conn.Dispose();
            _db.Dispose();
        }

        private static void Compare(IDbCommand output, (string name, object value)[] expected)
        {
            var parms = output.Parameters.Cast<SqlParameter>();
            parms.Count().ShouldBe(expected.Count());
            foreach (var (name, value) in expected)
            {
                parms.ShouldContain(p => p.ParameterName == name && p.Value.Equals(value));
            }
        }

        [Theory]
        [MemberData(nameof(QueryParamData))]
        public void QueryWithParams_Should_Match(string sql, object[] args, (string, object)[] expected)
        {
            var output = _db.CreateCommand(_conn, sql, args);
            output.CommandType.ShouldBe(CommandType.Text);
            output.CommandText.ShouldBe(sql);
            Compare(output, expected);
        }

        [Fact]
        public void QueryWithNamedParams_Should_Match()
        {
            var inputSql = "select * from foo where bar=@thing1";
            var args = new { thing1 = "baz" };
            var expectedSql = "select * from foo where bar=@0";
            var expectedArgs = new (string, object)[] { ("@0", "baz") };

            var output = _db.CreateCommand(_conn, inputSql, args);
            output.CommandType.ShouldBe(CommandType.Text);
            output.CommandText.ShouldBe(expectedSql);
            Compare(output, expectedArgs);
        }

        [Fact]
        public void QueryWithSql_Should_Work()
        {
            var sql = Sql.Builder.Select("*").From("SomeTable");
            sql.Where("foo=@0", "thing1");
            sql.Where("bar=@0", 4);
            var output = _db.CreateCommand(_conn, sql.SQL, sql.Arguments);

            output.CommandType.ShouldBe(CommandType.Text);
            output.CommandText.ShouldBe("SELECT *\nFROM SomeTable\nWHERE (foo=@0)\nAND (bar=@1)");
            var expected = new (string, object)[] { ("@0", "thing1"), ("@1", 4) };
            Compare(output, expected);
        }

        [Theory]
        [MemberData(nameof(ProcParamData))]
        public void ProcWithParams_Should_Match(string sql, object[] args, (string, object)[] expected)
        {
            var output = _db.CreateCommand(_conn, CommandType.StoredProcedure, sql, args);
            output.CommandType.ShouldBe(CommandType.StoredProcedure);
            output.CommandText.ShouldBe(sql);
            Compare(output, expected);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(4)]
        public void ValueTypes_Should_Throw(object arg)
        {
            Action act = () => _db.CreateCommand(_conn, CommandType.StoredProcedure, "procname", arg);
            act.ShouldThrow<ArgumentException>();
        }
    }
}