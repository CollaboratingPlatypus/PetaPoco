using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Utilities
{
    public partial class PagingHelperTests
    {
        [Fact]
        public void SplitSQL_GivenSqlWithoutOrderBy_ShouldLeaveSqlOrderByUnset()
        {
            var parts = Split("SELECT * FROM tbl");

            parts.SqlOrderBy.ShouldBeNull();
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithOrderBy_ShouldExtractTheOrderByClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithMultiColumnOrderBy_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 ASC, col2 DESC");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1 ASC, col2 DESC");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithAggregateOrderBy_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY SUM(col1) ASC, col2");

            parts.SqlOrderBy.ShouldBe("ORDER BY SUM(col1) ASC, col2");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithColumnOrdinals_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY 1, 2");

            parts.SqlOrderBy.ShouldBe("ORDER BY 1, 2");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithSubqueryOrderByExpression_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY (SELECT NULL)");

            parts.SqlOrderBy.ShouldBe("ORDER BY (SELECT NULL)");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithCaseOrderByExpression_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY CASE WHEN a = 1 THEN 1 ELSE 2 END");

            parts.SqlOrderBy.ShouldBe("ORDER BY CASE WHEN a = 1 THEN 1 ELSE 2 END");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithEscapedOrderByColumns_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl t ORDER BY t.[col1], [col2] DESC, `col3`");

            parts.SqlOrderBy.ShouldBe("ORDER BY t.[col1], [col2] DESC, `col3`");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl t");
        }

        [Fact]
        public void SplitSQL_GivenOrderBySpreadOverMultipleLines_ShouldExtractTheWholeClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER\nBY col1,\n\tcol2 DESC");

            Normalize(parts.SqlOrderBy).ShouldBe("ORDER BY col1, col2 DESC");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenOrderByTerminatedBySemicolon_ShouldKeepTheTerminatorOutOfTheClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1;");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByWhitespace_ShouldNotIncludeTheWhitespaceInTheClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1\n");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByStatementClause_ShouldExcludeTheClauseFromSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 LIMIT 10");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
            Normalize(parts.SqlSelectRemoved).ShouldBe("* FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithLimitClause_ShouldNotIncludeTheLimitInTheCountStatement()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 LIMIT 10");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithOffsetFetchClause_ShouldNotIncludeItInTheCountStatement()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithForUpdateClause_ShouldNotIncludeItInTheCountStatement()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 FOR UPDATE");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenGroupBySqlWithLimitClause_ShouldNotIncludeTheLimitInTheCountStatement()
        {
            var parts = Split("SELECT col1 FROM tbl GROUP BY col1 ORDER BY col1 LIMIT 10");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT col1 FROM tbl GROUP BY col1) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenLimitInsideSubquery_ShouldKeepTheSubqueryLimitInTheCountStatement()
        {
            var parts = Split("SELECT * FROM (SELECT * FROM t LIMIT 10) x ORDER BY a");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT * FROM t LIMIT 10) x");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByComment_ShouldExcludeTheCommentFromSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 -- comment");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByOffsetFetch_ShouldExcludeItFromSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByForUpdate_ShouldExcludeItFromSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 FOR UPDATE");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
        }

        [Fact]
        public void SplitSQL_GivenOrderByFollowedByLimitAll_ShouldExcludeItFromSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 LIMIT ALL");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
        }

        [Fact]
        public void SplitSQL_GivenOrderByWithCollation_ShouldKeepTheCollationInSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY col1 COLLATE Latin1_General_CI_AS");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1 COLLATE Latin1_General_CI_AS");
        }

        [Fact]
        public void SplitSQL_GivenColumnsNamedAfterStatementClauseKeywords_ShouldKeepThemInSqlOrderBy()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY limit DESC, offset, t.limit, [limit]");

            parts.SqlOrderBy.ShouldBe("ORDER BY limit DESC, offset, t.limit, [limit]");
        }

        [Fact]
        public void SplitSQL_GivenOrderBy_ShouldExcludeTheClauseFromSqlSelectRemoved()
        {
            var parts = Split("SELECT a, b FROM tbl WHERE a > 1 ORDER BY a");

            parts.SqlOrderBy.ShouldBe("ORDER BY a");
            Normalize(parts.SqlSelectRemoved).ShouldBe("a, b FROM tbl WHERE a > 1");
        }

        [Fact]
        public void SplitSQL_GivenOrderByOnlyInsideSubquery_ShouldLeaveSqlOrderByUnset()
        {
            var parts = Split("SELECT * FROM (SELECT * FROM t ORDER BY a) x");

            parts.SqlOrderBy.ShouldBeNull();
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT * FROM t ORDER BY a) x");
        }

        [Fact]
        public void SplitSQL_GivenOrderByInsideSubqueryAndAtTopLevel_ShouldExtractTheTopLevelClause()
        {
            var parts = Split("SELECT * FROM (SELECT * FROM t ORDER BY a) AS x ORDER BY b");

            parts.SqlOrderBy.ShouldBe("ORDER BY b");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT * FROM t ORDER BY a) AS x");
        }

        [Fact]
        public void SplitSQL_GivenOrderByContainingNestedOrderBy_ShouldExtractTheOuterClause()
        {
            var parts = Split("SELECT * FROM t ORDER BY a DESC, (SELECT MAX(x) FROM u ORDER BY y) ASC");

            parts.SqlOrderBy.ShouldBe("ORDER BY a DESC, (SELECT MAX(x) FROM u ORDER BY y) ASC");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM t");
        }

        [Fact]
        public void SplitSQL_GivenOrderByKeywordInsideStringLiteral_ShouldNotTreatItAsAClause()
        {
            var parts = Split("SELECT * FROM tbl WHERE col1 = 'ORDER BY col2'");

            parts.SqlOrderBy.ShouldBeNull();
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl WHERE col1 = 'ORDER BY col2'");
        }

        [Fact]
        public void SplitSQL_GivenColumnNameStartingWithOrderByKeyword_ShouldNotBeConfusedWithAClause()
        {
            var parts = Split("SELECT * FROM tbl ORDER BY ordering");

            parts.SqlOrderBy.ShouldBe("ORDER BY ordering");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenColumnNameContainingOrderByKeyword_ShouldNotBeConfusedWithAClause()
        {
            var parts = Split("SELECT * FROM tbl WHERE ordering = 1");

            parts.SqlOrderBy.ShouldBeNull();
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl WHERE ordering = 1");
        }

        [Fact]
        public void SplitSQL_GivenDistinctSqlWithOrderBy_ShouldWrapTheCountStatement()
        {
            var parts = Split("SELECT DISTINCT col1 FROM tbl ORDER BY col1");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT DISTINCT col1 FROM tbl) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenGroupBySqlWithOrderBy_ShouldWrapTheCountStatement()
        {
            var parts = Split("SELECT col1 FROM tbl GROUP BY col1 ORDER BY col1");

            parts.SqlOrderBy.ShouldBe("ORDER BY col1");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT col1 FROM tbl GROUP BY col1) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithNestedOrderByClauses_ShouldOnlyExtractTheTopLevelClause()
        {
            var parts = Split(
                "SELECT t.Id, (SELECT MAX(x) FROM u WHERE u.Id = t.Id ORDER BY u.Id DESC) AS MaxX\n" +
                "FROM t\n" +
                "WHERE t.Id IN (SELECT Id FROM v ORDER BY Id)\n" +
                "ORDER BY t.Id DESC");

            parts.SqlOrderBy.ShouldBe("ORDER BY t.Id DESC");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM t WHERE t.Id IN (SELECT Id FROM v ORDER BY Id)");
        }

        [Fact]
        public void SplitSQL_GivenTopLevelGroupBy_ShouldWrapTheCountStatement()
        {
            var parts = Split("SELECT a, COUNT(*) FROM tbl GROUP BY a");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT a, COUNT(*) FROM tbl GROUP BY a) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenTopLevelGroupByAndOrderBy_ShouldWrapTheCountStatement()
        {
            var parts = Split("SELECT a, COUNT(*) FROM tbl GROUP BY a ORDER BY a");

            parts.SqlOrderBy.ShouldBe("ORDER BY a");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT a, COUNT(*) FROM tbl GROUP BY a) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenGroupByOnlyInsideSubquery_ShouldNotWrapTheCountStatement()
        {
            var parts = Split("SELECT a FROM (SELECT b FROM u GROUP BY b) AS x");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT b FROM u GROUP BY b) AS x");
        }

        [Fact]
        public void SplitSQL_GivenGroupByOnlyInsideSelectExpression_ShouldNotWrapTheCountStatement()
        {
            var parts = Split("SELECT a, (SELECT COUNT(*) FROM u GROUP BY x) AS c FROM t");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM t");
        }

        [Fact]
        public void SplitSQL_GivenGroupByKeywordInsideStringLiteral_ShouldNotWrapTheCountStatement()
        {
            var parts = Split("SELECT a FROM tbl WHERE a = 'GROUP BY x'");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl WHERE a = 'GROUP BY x'");
        }

        [Fact]
        public void SplitSQL_GivenGroupByKeywordInsideComment_ShouldNotWrapTheCountStatement()
        {
            var parts = Split("SELECT a FROM tbl -- GROUP BY a");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl -- GROUP BY a");
        }

        [Fact]
        public void SplitSQL_GivenOrderByKeywordInsideComment_ShouldNotBeUsedAsTheClause()
        {
            var parts = Split("SELECT a FROM tbl ORDER BY a -- ORDER BY bogus");

            parts.SqlOrderBy.ShouldBe("ORDER BY a");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenTerminatedSql_ShouldKeepTheTerminatorOutOfTheGeneratedStatements()
        {
            var parts = Split("SELECT a FROM tbl;");

            Normalize(parts.SqlSelectRemoved).ShouldBe("a FROM tbl");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenTerminatedDistinctSql_ShouldNotPutTheTerminatorInsideTheDerivedTable()
        {
            var parts = Split("SELECT DISTINCT a FROM tbl;");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT DISTINCT a FROM tbl) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenTerminatedGroupBySql_ShouldNotPutTheTerminatorInsideTheDerivedTable()
        {
            var parts = Split("SELECT a, COUNT(*) FROM tbl GROUP BY a;");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM (SELECT a, COUNT(*) FROM tbl GROUP BY a) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithLeadingWhitespaceAndTerminator_ShouldNotCarryThemIntoTheCountStatement()
        {
            var parts = Split("  ; SELECT a FROM tbl");

            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithoutWhitespaceAfterTheSelectKeyword_ShouldSplitIt()
        {
            var parts = Split("SELECT*FROM tbl");

            Normalize(parts.SqlSelectRemoved).ShouldBe("*FROM tbl");
            Normalize(parts.SqlCount).ShouldBe("SELECT COUNT(*) FROM tbl");
        }

        [Fact]
        public void SplitSQL_GivenCte_ShouldKeepTheCteInTheCountStatement()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT b FROM cte");

            parts.SqlCte.ShouldBe("WITH cte AS (SELECT a FROM t) ");
            Normalize(parts.SqlCount).ShouldBe("WITH cte AS (SELECT a FROM t) SELECT COUNT(*) FROM cte");
        }

        [Fact]
        public void SplitSQL_GivenCte_ShouldExcludeTheCteFromSqlSelectRemoved()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT b FROM cte");

            Normalize(parts.SqlSelectRemoved).ShouldBe("b FROM cte");
        }

        [Fact]
        public void SplitSQL_GivenCteWithOrderBy_ShouldKeepTheCteAndExcludeTheOrderBy()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT b FROM cte ORDER BY b");

            parts.SqlOrderBy.ShouldBe("ORDER BY b");
            Normalize(parts.SqlCount).ShouldBe("WITH cte AS (SELECT a FROM t) SELECT COUNT(*) FROM cte");
        }

        [Fact]
        public void SplitSQL_GivenCteWithTrailingStatementClause_ShouldExcludeItFromTheCountStatement()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT b FROM cte ORDER BY b LIMIT 10");

            parts.SqlOrderBy.ShouldBe("ORDER BY b");
            Normalize(parts.SqlCount).ShouldBe("WITH cte AS (SELECT a FROM t) SELECT COUNT(*) FROM cte");
        }

        [Fact]
        public void SplitSQL_GivenCteWithDistinct_ShouldKeepTheCteInFrontOfTheWrappedCountStatement()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT DISTINCT b FROM cte");

            parts.IsDistinct.ShouldBeTrue();
            Normalize(parts.SqlCount).ShouldBe(
                "WITH cte AS (SELECT a FROM t) SELECT COUNT(*) FROM (SELECT DISTINCT b FROM cte) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenCteWithGroupBy_ShouldKeepTheCteInFrontOfTheWrappedCountStatement()
        {
            var parts = Split("WITH cte AS (SELECT a FROM t) SELECT b, COUNT(*) FROM cte GROUP BY b");

            Normalize(parts.SqlCount).ShouldBe(
                "WITH cte AS (SELECT a FROM t) SELECT COUNT(*) FROM (SELECT b, COUNT(*) FROM cte GROUP BY b) countAlias");
        }

        [Fact]
        public void SplitSQL_GivenSqlWithoutCte_ShouldLeaveSqlCteUnset()
        {
            Split("SELECT a FROM tbl").SqlCte.ShouldBeNull();
        }
    }
}
