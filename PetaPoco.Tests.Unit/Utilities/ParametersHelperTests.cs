using Moq;
using PetaPoco.Core;
using PetaPoco.Internal;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PetaPoco.Tests.Unit.Utilities
{
    public class ParametersHelperTests
    {
        [Fact]
        public void EnsureParamPrefix_int_ShouldWork()
        {
            int input = 34;
            string output = input.EnsureParamPrefix("@");
            output.ShouldBe("@34");
        }

        [Theory]
        [InlineData("@foo")]
        [InlineData("$foo")]
        [InlineData(":foo")]
        [InlineData("^foo")]
        [InlineData("foo")]
        public void EnsureParamPrefix_string_ShouldWork(string input)
        {
            string output = input.EnsureParamPrefix("@");
            output.ShouldBe("@foo");
        }

        [Theory]
        [InlineData(":")]
        [InlineData("^")]
        public void ReplaceParamPrefix_ShouldWork(string prefix)
        {
            var input = "This @foo is @bar";
            var expected = $"This {prefix}foo is {prefix}bar";
            var output = input.ReplaceParamPrefix(prefix);
            output.ShouldBe(expected);
        }

        [Fact]
        public void ProcessQueryParams_PositionalParams_ShouldWork()
        {
            var sql = "select * from foo where a = @0 and b = @1";
            var args_src = new object[] { 5, "bird" };
            var args_dest = new List<object>();
            var output = ParametersHelper.ProcessQueryParams(sql, args_src, args_dest);

            output.ShouldBe(sql);
            args_dest.ShouldBe(args_src.ToList());
        }

        private void NamedQueryParamsTestHelper(object[] args_src)
        {
            var sql = "select * from foo where a = @first and b = @second";
            var args_dest = new List<object>();

            var expected_sql = "select * from foo where a = @0 and b = @1";
            var expected_args = new List<object>() { 87, "Bob" };

            var output = ParametersHelper.ProcessQueryParams(sql, args_src, args_dest);

            output.ShouldBe(expected_sql);
            args_dest.ShouldBe(expected_args);
        }

        [Fact]
        public void ProcessQueryParams_ObjectWithProperties_ShouldWork()
        {
            var args_src = new[] { new { second = "Bob", first = 87 } };
            NamedQueryParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessQueryParams_Dictionary_ShouldWork()
        {
            var args_src = new[] { new Dictionary<string, object> { ["second"] = "Bob", ["first"] = 87 } };
            NamedQueryParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessQueryParams_DictionaryAndObject_ShouldWork()
        {
            var args_src = new object[] { new Dictionary<string, object> { ["second"] = "Bob" }, new { first = 87 } };
            NamedQueryParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessQueryParams_ObjectMissingParam_ShouldThrow()
        {
            var args_src = new[] { new {first = 87 } };
            Action act = () => NamedQueryParamsTestHelper(args_src);

            var ex = act.ShouldThrow<ArgumentException>();
            ex.Message.ShouldMatch(@"^Parameter '@second' specified");
        }

        [Fact]
        public void ProcessQueryParams_DictionaryMissingParam_ShouldThrow()
        {
            var args_src = new[] { new Dictionary<string, object>() { ["first"] = 87 } };
            Action act = () => NamedQueryParamsTestHelper(args_src);

            var ex = act.ShouldThrow<ArgumentException>();
            ex.Message.ShouldMatch(@"^Parameter '@second' specified");
        }

        [Fact]
        public void ProcessQueryParams_ObjectWithNull_ShouldWork()
        {
            var sql = "select * from foo where a = @first";
            var args_src = new[] { new { first = (string)null } };
            var args_dest = new List<object>();

            var expected_sql = "select * from foo where a = @0";
            var expected_args = new List<object>() { null };

            var output = ParametersHelper.ProcessQueryParams(sql, args_src, args_dest);

            output.ShouldBe(expected_sql);
            args_dest.ShouldBe(expected_args);
        }

        [Fact]
        public void ProcessQueryParams_DictionaryWithNull_ShouldWork()
        {
            var sql = "select * from foo where a = @first";
            var args_src = new[] { new Dictionary<string, object>() { ["first"] = null } };
            var args_dest = new List<object>();

            var expected_sql = "select * from foo where a = @0";
            var expected_args = new List<object>() { null };

            var output = ParametersHelper.ProcessQueryParams(sql, args_src, args_dest);

            output.ShouldBe(expected_sql);
            args_dest.ShouldBe(expected_args);
        }

        private void NamedProcParamsTestHelper(object[] args_src)
        {
            Action<IDbDataParameter, object, PocoColumn> setAction = (p, o, pc) => p.Value = o;
            var cmd = new SqlCommand();

            var expected = new [] { new SqlParameter("foo", 42), new SqlParameter("bar", "Dirk Gently") };
            var output = ParametersHelper.ProcessStoredProcParams(cmd, args_src, setAction)
                .Cast<IDbDataParameter>()
                .ToArray();

            output.Count().ShouldBe(2);
            output[0].ParameterName.ShouldBe(expected[0].ParameterName);
            output[0].Value.ShouldBe(expected[0].Value);
            output[1].ParameterName.ShouldBe(expected[1].ParameterName);
            output[1].Value.ShouldBe(expected[1].Value);

        }

        [Fact]
        public void ProcessStoredProcParams_Parameter_ShouldWork()
        {
            var args_src = new object[] { new SqlParameter("foo", 42), new SqlParameter("bar", "Dirk Gently") };
            NamedProcParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessStoredProcParams_ObjectWithProperties_ShouldWork()
        {
            var args_src = new object[] { new { foo = 42, bar = "Dirk Gently" } };
            NamedProcParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessStoredProcParams_Dictionary_ShouldWork()
        {
            var args_src = new[] { new Dictionary<string, object>() { ["foo"] = 42, ["bar"] = "Dirk Gently" } };
            NamedProcParamsTestHelper(args_src);
        }

        [Fact]
        public void ProcessStoredProcParams_DictionaryAndObject_ShouldWork()
        {
            var args_src = new object[] { new Dictionary<string, object>() { ["foo"] = 42 }, new { bar = "Dirk Gently" } };
            NamedProcParamsTestHelper(args_src);
        }
    }
}