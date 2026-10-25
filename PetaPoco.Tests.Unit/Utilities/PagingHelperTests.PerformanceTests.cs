using System.Linq;
using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Utilities
{
    public partial class PagingHelperTests
    {
        [Fact]
        public void SplitSQL_GivenComplexSqlStringWithOrWithoutCertainClauses_ShouldPerformTheSameInAllCases()
        {
            var complexSqlNoDistinct = string.Format(ComplexSqlTemplate, string.Empty);
            var complexSqlWithDistinct = string.Format(ComplexSqlTemplate, "DISTINCT");

            // Declaring the complex statement as a common table expression exercises the scan of the common table expression
            // list in addition to the scan of the outermost statement. The clauses that are appended to it below are only
            // ever parsed, so they do not have to resolve against the tables of the statement.
            var complexCteSql = $"WITH cte AS (\n{complexSqlNoDistinct}\n) SELECT cte.SalesYear FROM cte";

            var sqlList = new string[]
            {
                complexSqlNoDistinct,
                complexSqlNoDistinct + GroupByClause,
                complexSqlNoDistinct + OrderByClause,
                complexSqlNoDistinct + GroupByClause + OrderByClause,

                complexSqlWithDistinct,
                complexSqlWithDistinct + GroupByClause,
                complexSqlWithDistinct + OrderByClause,
                complexSqlWithDistinct + GroupByClause + OrderByClause,

                complexCteSql,
                complexCteSql + GroupByClause,
                complexCteSql + OrderByClause,
                complexCteSql + GroupByClause + OrderByClause
            };

            _ = Split(complexSqlNoDistinct); //Warmup

            var results = sqlList.Select(SplitWithDuration).ToList();
            var durations = results.Select(o => o.Duration.TotalMilliseconds).ToList();

            var maxThresholdMs = 100.0;
            var actualMinMs = durations.Min();
            var actualMaxMs = durations.Max();

            var maxSpreadMs = 50.0; // Max allowed gap between fastest and slowest
            var actualSpreadMs = actualMaxMs - actualMinMs;

            actualMaxMs.ShouldBeLessThanOrEqualTo(maxThresholdMs, () =>
                $"{nameof(PagingHelper.SplitSQL)} degrades in performance. " +
                $"Expected the slowest execution to be {maxThresholdMs}ms or less, but was {actualMaxMs}ms.");

            actualSpreadMs.ShouldBeLessThanOrEqualTo(maxSpreadMs, () =>
                $"{nameof(PagingHelper.SplitSQL)} has unstable performance. " +
                $"Expected execution times to be within {maxSpreadMs}ms of each other, but the gap was {actualSpreadMs}ms " +
                $"(Fastest: {actualMinMs}ms, Slowest: {actualMaxMs}ms).");
        }
    }
}
