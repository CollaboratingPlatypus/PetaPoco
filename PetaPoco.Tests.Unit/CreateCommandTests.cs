using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using PetaPoco.Providers;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace PetaPoco.Tests.Unit
{
    public class CreateCommandTests: IDisposable
    {
        public readonly Database _db = new Database("foo", "System.Data.SqlClient");
        public readonly DbConnection _conn = new SqlConnection();

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

        public static IEnumerable<object[]> QueryParamData => new[]
        {
            new object[] {
                "select * from foo where bar=@0",
                new object[] { "baz" },
                new (string, object)[] { ("@0", "baz") }
            },
            new object[] {
                "select * from MyTable where bar=@0 and baz=@1",
                new object[] { 3, 7 },
                new (string, object)[] { ("@0", 3), ("@1", 7) }
            },
            new object[] {
                "select * from foo",
                new object[0],
                new (string, object)[0]
            },
        };

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

        public static IEnumerable<object[]> ProcParamData => new[]
        {
            new object[] {
                "procname",
                new object[] { new { Foo = "Bar", Baz = 3 } },
                new (string, object)[] { ("@Foo", "Bar"), ("@Baz", 3) }
            },
            new object[] {
                "8B182821-6BFE-4F00-8402-AC0278019DD2",
                new object[] { new { Foo = "Bar" }, new { Baz = 3 } },
                new (string, object)[] { ("@Foo", "Bar"), ("@Baz", 3) }
            },
            new object[] {
                "F5C94316-5040-4FF6-A0DA-86D96E96B8AE",
                new object[] { new SqlParameter("@Foo", "Bar") },
                new (string, object)[] { ("@Foo", "Bar") }
            },
            new object[] {
                "B5294FE8-766F-4D4B-A47B-EA82CC4F748B",
                new object[0],
                new (string, object)[0]
            },
        };

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
