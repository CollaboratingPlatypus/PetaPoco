using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Utilities
{
    /// <summary>
    /// Tests for <see cref="SqlClauseScanner.Analyze"/>, which locates the outermost <c>SELECT</c> statement of an SQL statement
    /// and the clauses that are relevant to paging it.
    /// </summary>
    public class SqlClauseScannerTests
    {
        private static SqlAnalysis Analyze(string sql) => SqlClauseScanner.Analyze(sql);

        [Fact]
        public void Analyze_GivenSelectWithoutCte_ShouldLocateTheOutermostSelect()
        {
            var sql = "SELECT a FROM t";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.Cte.ShouldBeNull();
            analysis.SelectIndex.ShouldBe(0);
            analysis.SelectListStart.ShouldBe(7);
            analysis.SelectListEnd.ShouldBe(9);
            analysis.StatementEnd.ShouldBe(sql.Length);
        }

        [Fact]
        public void Analyze_GivenLeadingWhitespaceAndComments_ShouldSkipThem()
        {
            var sql = "  /* leading */ -- line\n\tSELECT a FROM t";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            sql.Substring(analysis.SelectIndex, 6).ShouldBe("SELECT");
            sql.Substring(analysis.SelectListStart, 1).ShouldBe("a");
        }

        [Fact]
        public void Analyze_GivenLeadingEmptyStatement_ShouldSkipIt()
        {
            var sql = "; SELECT a FROM t";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            sql.Substring(analysis.SelectIndex, 6).ShouldBe("SELECT");
        }

        [Fact]
        public void Analyze_GivenStatementTerminator_ShouldLocateIt()
        {
            var sql = "SELECT a FROM t;";
            var analysis = Analyze(sql);

            analysis.StatementEnd.ShouldBe(sql.Length - 1);
            sql[analysis.StatementEnd].ShouldBe(';');
        }

        [Fact]
        public void Analyze_GivenStatementWithoutFrom_ShouldNotSucceed()
        {
            var analysis = Analyze("SELECT 1");

            analysis.Success.ShouldBeFalse();
            analysis.SelectListEnd.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenStatementThatIsNotASelect_ShouldNotSucceed()
        {
            Analyze("UPDATE t SET a = 1").Success.ShouldBeFalse();
            Analyze("INSERT INTO t VALUES (1)").Success.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenEmptyStatement_ShouldNotSucceed()
        {
            Analyze(null).Success.ShouldBeFalse();
            Analyze(string.Empty).Success.ShouldBeFalse();
            Analyze("   ").Success.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenSelectList_ShouldEndItAtTheTopLevelFrom()
        {
            var sql = "SELECT a, (SELECT b FROM u) AS c FROM t";
            var analysis = Analyze(sql);

            analysis.SelectListEnd.ShouldBe(sql.LastIndexOf("FROM", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenFromKeywordInsideLiteral_ShouldNotEndTheSelectList()
        {
            var sql = "SELECT 'FROM' AS a FROM t";
            var analysis = Analyze(sql);

            analysis.SelectListEnd.ShouldBe(sql.LastIndexOf("FROM", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenColumnsNamedAfterTheFromKeyword_ShouldNotEndTheSelectList()
        {
            var sql = "SELECT fromage, from_tbl FROM t";
            var analysis = Analyze(sql);

            analysis.SelectListEnd.ShouldBe(sql.LastIndexOf("FROM", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenDistinctSelect_ShouldFlagDistinct()
        {
            Analyze("SELECT DISTINCT a FROM t").IsDistinct.ShouldBeTrue();
        }

        [Fact]
        public void Analyze_GivenNonDistinctSelect_ShouldNotFlagDistinct()
        {
            Analyze("SELECT a FROM t").IsDistinct.ShouldBeFalse();
            Analyze("SELECT ALL a FROM t").IsDistinct.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenColumnStartingWithTheDistinctKeyword_ShouldNotFlagDistinct()
        {
            Analyze("SELECT distinct_col FROM t").IsDistinct.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenDistinctInsideSubquery_ShouldNotFlagDistinct()
        {
            Analyze("SELECT a FROM (SELECT DISTINCT b FROM u) AS x").IsDistinct.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenDistinctWithoutWhitespace_ShouldFlagDistinct()
        {
            // The keyword is recognised on a keyword boundary, so "DISTINCT(a)" is the distinct modifier and not a column name.
            Analyze("SELECT DISTINCT(a) FROM t").IsDistinct.ShouldBeTrue();
        }

        [Fact]
        public void Analyze_GivenTopLevelGroupBy_ShouldLocateIt()
        {
            var sql = "SELECT a, COUNT(*) FROM t GROUP BY a";
            var analysis = Analyze(sql);

            sql.Substring(analysis.GroupByIndex, 8).ShouldBe("GROUP BY");
            analysis.GroupByIndex.ShouldBe(sql.IndexOf("GROUP BY", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenNoGroupBy_ShouldNotLocateIt()
        {
            Analyze("SELECT a FROM t ORDER BY a").GroupByIndex.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenGroupByInsideSubquery_ShouldNotLocateIt()
        {
            var sql = "SELECT a, (SELECT b FROM u GROUP BY b) AS c FROM t GROUP BY a";
            var analysis = Analyze(sql);

            analysis.GroupByIndex.ShouldBe(sql.LastIndexOf("GROUP BY", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenGroupByKeywordInsideLiteralOrComment_ShouldNotLocateIt()
        {
            Analyze("SELECT a FROM t WHERE a = 'GROUP BY x'").GroupByIndex.ShouldBe(-1);
            Analyze("SELECT a FROM t -- GROUP BY a").GroupByIndex.ShouldBe(-1);
            Analyze("SELECT a FROM t /* GROUP BY a */").GroupByIndex.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenGroupBySpreadOverWhitespace_ShouldLocateIt()
        {
            var sql = "SELECT a FROM t GROUP\n\tBY a";
            var analysis = Analyze(sql);

            analysis.GroupByIndex.ShouldBe(sql.IndexOf("GROUP", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenTopLevelOrderBy_ShouldLocateTheClauseAndItsEnd()
        {
            var sql = "SELECT a FROM t ORDER BY a";
            var analysis = Analyze(sql);

            sql.Substring(analysis.OrderByIndex, 8).ShouldBe("ORDER BY");
            analysis.OrderByEnd.ShouldBe(sql.Length);
        }

        [Fact]
        public void Analyze_GivenNoOrderBy_ShouldNotLocateIt()
        {
            var analysis = Analyze("SELECT a FROM t");

            analysis.OrderByIndex.ShouldBe(-1);
            analysis.OrderByEnd.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenOrderByFollowedByStatementClause_ShouldEndTheClauseAtTheClause()
        {
            var sql = "SELECT a FROM t ORDER BY a LIMIT 10";
            Analyze(sql).OrderByEnd.ShouldBe(sql.IndexOf("LIMIT", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenOrderByFollowedByForUpdate_ShouldEndTheClauseAtTheClause()
        {
            var sql = "SELECT a FROM t ORDER BY a FOR UPDATE";
            Analyze(sql).OrderByEnd.ShouldBe(sql.IndexOf("FOR", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenOrderByFollowedByComment_ShouldEndTheClauseAtTheComment()
        {
            var sql = "SELECT a FROM t ORDER BY a -- comment";
            Analyze(sql).OrderByEnd.ShouldBe(sql.IndexOf("--", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenOrderByFollowedByTerminator_ShouldEndTheClauseAtTheTerminator()
        {
            var sql = "SELECT a FROM t ORDER BY a;";
            Analyze(sql).OrderByEnd.ShouldBe(sql.IndexOf(';'));
        }

        [Fact]
        public void Analyze_GivenOrderByWithoutExpression_ShouldNotProduceAnEnd()
        {
            var sql = "SELECT a FROM t ORDER BY LIMIT 10";
            var analysis = Analyze(sql);

            analysis.OrderByIndex.ShouldBe(sql.IndexOf("ORDER", System.StringComparison.Ordinal));
            analysis.OrderByEnd.ShouldBe(-1);

            // Without the whitespace that the keyword pair requires there is no clause at all.
            Analyze("SELECT a FROM t ORDER BY").OrderByIndex.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenOrderByInsideSubquery_ShouldNotLocateIt()
        {
            Analyze("SELECT a FROM (SELECT b FROM u ORDER BY b) AS x").OrderByIndex.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenOrderByContainingNestedOrderBy_ShouldEndAtTheOuterEnd()
        {
            var sql = "SELECT a FROM t ORDER BY a DESC, (SELECT b FROM u ORDER BY c) ASC";
            Analyze(sql).OrderByEnd.ShouldBe(sql.Length);
        }

        [Fact]
        public void Analyze_GivenOrderByColumnsNamedAfterTrailingClauseKeywords_ShouldNotEndTheClause()
        {
            var sql = "SELECT a FROM t ORDER BY limit DESC, offset, t.limit, [limit]";
            Analyze(sql).OrderByEnd.ShouldBe(sql.Length);
        }

        [Fact]
        public void Analyze_GivenOrderBySpreadOverWhitespace_ShouldLocateIt()
        {
            var sql = "SELECT a FROM t ORDER\nBY a";
            var analysis = Analyze(sql);

            analysis.OrderByIndex.ShouldBe(sql.IndexOf("ORDER", System.StringComparison.Ordinal));
            analysis.OrderByEnd.ShouldBe(sql.Length);
        }

        [Fact]
        public void Analyze_GivenOrderByKeywordInsideLiteral_ShouldNotLocateIt()
        {
            Analyze("SELECT a FROM t WHERE a = 'ORDER BY b'").OrderByIndex.ShouldBe(-1);
        }

        [Fact]
        public void Analyze_GivenSingleCte_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH cte AS (SELECT a FROM t) SELECT b FROM cte";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.Cte.ShouldBe("WITH cte AS (SELECT a FROM t) ");
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT b", System.StringComparison.Ordinal));
            analysis.SelectListEnd.ShouldBe(sql.IndexOf("FROM cte", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCte_ShouldProduceACteThatRebuildsTheStatement()
        {
            var sql = "WITH cte AS (SELECT a FROM t)\nSELECT b FROM cte ORDER BY b";
            var analysis = Analyze(sql);

            // The text of the common table expression list is what a derived statement has to keep in front of it.
            (analysis.Cte + sql.Substring(analysis.SelectIndex)).ShouldBe(sql);
        }

        [Fact]
        public void Analyze_GivenMultipleCtes_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH a AS (SELECT 1 AS x), b AS (SELECT x FROM a) SELECT x FROM b";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT x FROM b", System.StringComparison.Ordinal));
            (analysis.Cte + sql.Substring(analysis.SelectIndex)).ShouldBe(sql);
        }

        [Fact]
        public void Analyze_GivenRecursiveCte_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH RECURSIVE cte AS (SELECT 1 AS n UNION ALL SELECT n + 1 FROM cte WHERE n < 5) SELECT n FROM cte";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT n FROM cte", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCteWithColumnList_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH cte (x, y) AS (SELECT 1, 2) SELECT x FROM cte";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT x FROM cte", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCteWithQuotedName_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH [my cte] AS (SELECT 1 AS x) SELECT x FROM [my cte]";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT x FROM", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCteWithNestedParenthesesLiteralsAndComments_ShouldLocateTheOutermostSelect()
        {
            var sql = "WITH cte AS (SELECT ')' AS c, (1 + 2) AS n FROM t /* ) */) SELECT c FROM cte";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.SelectIndex.ShouldBe(sql.IndexOf("SELECT c FROM", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCteBodyWithGroupByAndOrderBy_ShouldIgnoreThem()
        {
            var sql = "WITH cte AS (SELECT a, COUNT(*) FROM t GROUP BY a ORDER BY a) SELECT a FROM cte ORDER BY a";
            var analysis = Analyze(sql);

            analysis.GroupByIndex.ShouldBe(-1);
            analysis.OrderByIndex.ShouldBe(sql.LastIndexOf("ORDER BY", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenCteAndTopLevelClauses_ShouldLocateThemAll()
        {
            var sql = "WITH cte AS (SELECT a FROM t GROUP BY a) SELECT DISTINCT a, COUNT(*) FROM cte GROUP BY a ORDER BY a LIMIT 10";
            var analysis = Analyze(sql);

            analysis.IsDistinct.ShouldBeTrue();
            analysis.GroupByIndex.ShouldBe(sql.LastIndexOf("GROUP BY", System.StringComparison.Ordinal));
            analysis.OrderByIndex.ShouldBe(sql.LastIndexOf("ORDER BY", System.StringComparison.Ordinal));
            analysis.OrderByEnd.ShouldBe(sql.IndexOf("LIMIT", System.StringComparison.Ordinal));
        }

        [Fact]
        public void Analyze_GivenTerminatorBeforeCte_ShouldLocateTheCte()
        {
            var sql = ";WITH cte AS (SELECT 1 AS x) SELECT x FROM cte";
            var analysis = Analyze(sql);

            analysis.Success.ShouldBeTrue();
            analysis.Cte.ShouldBe("WITH cte AS (SELECT 1 AS x) ");
        }

        [Fact]
        public void Analyze_GivenCommentsBetweenTheCteListAndTheSelect_ShouldKeepThemInTheCte()
        {
            var sql = "WITH cte AS (SELECT 1 AS x)\n/* comment */\nSELECT x FROM cte";
            var analysis = Analyze(sql);

            (analysis.Cte + sql.Substring(analysis.SelectIndex)).ShouldBe(sql);
            analysis.Cte.ShouldContain("/* comment */");
        }

        [Fact]
        public void Analyze_GivenCteWithDistinctInsideTheCteBody_ShouldNotFlagDistinct()
        {
            Analyze("WITH cte AS (SELECT DISTINCT a FROM t) SELECT a FROM cte").IsDistinct.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenCteFollowedByStatementThatIsNotASelect_ShouldNotSucceed()
        {
            Analyze("WITH cte AS (SELECT 1 AS x) DELETE FROM t WHERE x IN (SELECT x FROM cte)").Success.ShouldBeFalse();
        }

        [Fact]
        public void Analyze_GivenMalformedCte_ShouldNotSucceed()
        {
            Analyze("WITH cte SELECT a FROM t").Success.ShouldBeFalse();
            Analyze("WITH cte AS SELECT a FROM t").Success.ShouldBeFalse();
            Analyze("WITH cte AS (SELECT a FROM t").Success.ShouldBeFalse();
            Analyze("WITH AS (SELECT a FROM t) SELECT a FROM t").Success.ShouldBeFalse();
        }
    }
}
