using System;
using PetaPoco.Providers;
using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Providers
{
    public class OracleDatabaseProviderTests
    {
        private readonly OracleDatabaseProvider _provider = new OracleDatabaseProvider();

        [Fact]
        public void BuildPageQuery_GivenSqlWithOrderBy_ShouldReuseTheOrderByClause()
        {
            PagingHelper.Instance.SplitSQL("SELECT t.* FROM tbl t ORDER BY t.col1", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(10, 5, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY t.col1) peta_rn, t.* FROM tbl t) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
            args.ShouldBe(new object[] { 10, 15 });
        }

        [Fact]
        public void BuildPageQuery_GivenSqlWithOrderByAndTrailingStatementClause_ShouldNotIncludeTheClause()
        {
            PagingHelper.Instance.SplitSQL("SELECT t.* FROM tbl t ORDER BY t.col1 LIMIT 10", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(10, 5, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY t.col1) peta_rn, t.* FROM tbl t) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
            args.ShouldBe(new object[] { 10, 15 });
        }

        [Fact]
        public void BuildPageQuery_GivenSqlWithoutOrderBy_ShouldUseADefaultOrderBy()
        {
            PagingHelper.Instance.SplitSQL("SELECT t.* FROM tbl t", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL FROM DUAL)) peta_rn, t.* FROM tbl t) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }

        [Fact]
        public void BuildPageQuery_GivenSqlSelectingAllColumns_ShouldThrow()
        {
            PagingHelper.Instance.SplitSQL("SELECT * FROM tbl", out var parts);
            var args = new object[0];

            var exception = Should.Throw<Exception>(() => _provider.BuildPageQuery(0, 10, parts, ref args));

            exception.Message.ShouldContain("must alias");
        }

        [Fact]
        public void BuildPageQuery_GivenCte_ShouldKeepTheCteInFrontOfThePageQuery()
        {
            PagingHelper.Instance.SplitSQL("WITH cte AS (SELECT a FROM t) SELECT t.a FROM cte t ORDER BY t.a", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("WITH cte AS (SELECT a FROM t) SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY t.a) peta_rn, t.a FROM cte t) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }

        [Fact]
        public void BuildPageQuery_GivenCteWithDistinct_ShouldWrapTheDistinctSelectList()
        {
            PagingHelper.Instance.SplitSQL("WITH cte AS (SELECT a FROM t) SELECT DISTINCT t.a FROM cte t ORDER BY t.a", out var parts);
            var args = new object[0];

            var sql = _provider.BuildPageQuery(0, 10, parts, ref args);

            sql.ShouldBe("WITH cte AS (SELECT a FROM t) SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY t.a) peta_rn, peta_inner.* FROM (SELECT DISTINCT t.a FROM cte t) peta_inner) peta_paged WHERE peta_rn > @0 AND peta_rn <= @1");
        }
    }
}
