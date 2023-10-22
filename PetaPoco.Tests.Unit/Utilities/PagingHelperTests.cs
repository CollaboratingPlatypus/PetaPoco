using System;
using System.Diagnostics;
using System.Text;
using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Utilities
{
    public class PagingHelperTests
    {
        [Fact]
        public void SplitSQL_GivenBigSqlStringWithOrWithoutOrderBy_ShouldPerformTheSameInBothCases()
        {
            var inputWithOrderBy = GenerateSql(200, 5, 100, true);
            var inputWithoutOrderBy = inputWithOrderBy.Substring(0, inputWithOrderBy.IndexOf("ORDER BY"));

            var sw = Stopwatch.StartNew();

            PagingHelper.Instance.SplitSQL(inputWithOrderBy, out SQLParts partsWithOrderBy);
            sw.Stop();
            var elapsedMillSecondsWithOrderBy = sw.ElapsedMilliseconds;

            sw.Restart();

            PagingHelper.Instance.SplitSQL(inputWithoutOrderBy, out SQLParts partsWithoutOrderBy);
            sw.Stop();
            var elapsedMillSecondsWithoutOrderBy = sw.ElapsedMilliseconds;

            var actualTimeDiffInMilliSeconds = Math.Abs(elapsedMillSecondsWithOrderBy - elapsedMillSecondsWithoutOrderBy);
            var maxExpectedTimeDiffInMilliSeconds = 200;
            actualTimeDiffInMilliSeconds.ShouldBeLessThanOrEqualTo(maxExpectedTimeDiffInMilliSeconds,
                $"{nameof(PagingHelper.SplitSQL)} degrades in performance. Expected {maxExpectedTimeDiffInMilliSeconds}ms or less, but was {actualTimeDiffInMilliSeconds}ms.");
        }

        private static string GenerateSql(int numberOfColumns, int numberOfTables, int numberOfConditions, bool includeOrderBy)
        {
            var columns = GenerateSqlPart("column{0}", numberOfColumns, true);
            var tables = GenerateSqlPart("table{0}", numberOfTables, true);
            var conditions = GenerateSqlPart("AND column{0} = {0}", numberOfConditions, false);

            var builder = new StringBuilder();
            
            if (!string.IsNullOrEmpty(columns)) builder.Append($"SELECT {columns}");
            if (!string.IsNullOrEmpty(tables)) builder.Append($"FROM {tables}");
            if (!string.IsNullOrEmpty(conditions)) builder.Append($"WHERE {conditions}");

            if (includeOrderBy)
            {
                var sortColumns = GenerateSqlPart("column{0} ASC", 10, true);
                builder.Append($"ORDER BY {sortColumns}");
            }

            return builder.ToString();
        }

        private static string GenerateSqlPart(string template, int count, bool subIndent)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                if (i > 0 && subIndent) builder.Append("\t");
                builder.AppendLine(string.Format(template, i + 1));
            }

            return builder.ToString();
        }
    }
}
