using PetaPoco.Providers;
using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Providers
{
    public class SqlServerDatabaseProviderTests
    {
        private readonly SqlServerDatabaseProvider _provider = new SqlServerDatabaseProvider();

        [Theory]
        [InlineData("column.name", "[column.name]")]
        [InlineData("column name", "[column name]")]
        public void EscapeSqlIdentifier_GivenInput_ShouldBeExpected(string input, string expected)
        {
            _provider.EscapeSqlIdentifier(input).ShouldBe(expected);
        }

        [Theory]
        [InlineData("column.name", "column.name")]
        [InlineData("column name", "[column name]")]
        public void EscapeTableName_GivenInput_ShouldBeExpected(string input, string expected)
        {
            _provider.EscapeTableName(input).ShouldBe(expected);
        }

        [Fact]
        public void BuildPageQuery_GivenSqlWithOrderBy_ShouldReuseTheOrderByClause()
        {
            PagingHelper.Instance.SplitSQL("SELECT * FROM tbl ORDER BY col1", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(10, 5, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY col1) peta_rn, * FROM tbl) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
            args.ShouldBe(new object[] { 10, 15 });
        }

        [Fact]
        public void BuildPageQuery_GivenSqlWithOrderByAndTrailingStatementClause_ShouldNotIncludeTheClause()
        {
            PagingHelper.Instance.SplitSQL("SELECT * FROM tbl ORDER BY col1 LIMIT 10", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(10, 5, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY col1) peta_rn, * FROM tbl) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
            args.ShouldBe(new object[] { 10, 15 });
        }

        [Fact]
        public void BuildPageQuery_GivenSqlWithoutOrderBy_ShouldUseADefaultOrderBy()
        {
            PagingHelper.Instance.SplitSQL("SELECT * FROM tbl", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) peta_rn, * FROM tbl) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
            args.ShouldBe(new object[] { 0, 10 });
        }

        [Fact]
        public void BuildPageQuery_GivenCte_ShouldKeepTheCteInFrontOfThePageQuery()
        {
            PagingHelper.Instance.SplitSQL("WITH cte AS (SELECT a FROM t) SELECT a FROM cte ORDER BY a", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("WITH cte AS (SELECT a FROM t) SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY a) peta_rn, a FROM cte) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }

        [Fact]
        public void BuildPageQuery_GivenCteWithDistinct_ShouldWrapTheDistinctSelectList()
        {
            PagingHelper.Instance.SplitSQL("WITH cte AS (SELECT a FROM t) SELECT DISTINCT a FROM cte ORDER BY a", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("WITH cte AS (SELECT a FROM t) SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY a) peta_rn, peta_inner.* FROM (SELECT DISTINCT a FROM cte) peta_inner) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }

        [Fact]
        public void BuildPageQuery_GivenDistinctSelectWithoutWhitespace_ShouldWrapTheDistinctSelectList()
        {
            PagingHelper.Instance.SplitSQL("SELECT DISTINCT(a) FROM tbl ORDER BY a", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY a) peta_rn, peta_inner.* FROM (SELECT DISTINCT(a) FROM tbl) peta_inner) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }
    }
}
