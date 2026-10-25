using System;
using System.Text.RegularExpressions;

namespace PetaPoco.Utilities
{
    /// <summary>
    /// Provides utility methods for splitting SQL queries into parts and handling paging.
    /// </summary>
    public class PagingHelper : IPagingHelper
    {
        /// <summary>
        /// Gets the regular expression used for matching the <c>SELECT</c> clause.
        /// </summary>
        /// <remarks>
        /// <para>Beginning at the start of the string, this expression matches the keyword <c>SELECT</c> (inclusive), followed by one or
        /// more spaces, capturing everything in front of the word <c>FROM</c> (exclusive), accounting for special handling of nested
        /// parentheses, aggregate functions, etc.</para>
        /// <c><u>SELECT column1, column2</u> FROM tbl;</c><br/>
        /// <c><u>SELECT SUM(column1) AS sum_col1, column2</u> FROM tbl;</c>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex RegexColumns { get; } = new Regex(
            @"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>DISTINCT</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>Starting at the beginning of the string, this expression performs a simple match for the <c>DISTINCT</c> keyword in an SQL
        /// statement, followed by a space. Everything after is excluded.</para>
        /// <c>SELECT <u>DISTINCT</u> * FROM tbl;</c><br/>
        /// <c>SELECT <u>DISTINCT</u> column1 FROM tbl;</c>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex RegexDistinct { get; } = new Regex(@"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>ORDER BY</c> clause.
        /// </summary>
        /// <remarks>
        /// <para>This expression matches the <c>ORDER BY</c> clause, followed by one or more column names as well as the accompanying
        /// optional sort order modifier keywords, <c>ASC</c> and <c>DESC</c>, for each (inclusive). <c>AS</c> alias declarations are
        /// excluded from the match.</para>
        /// <c>SELECT * FROM tbl <u>ORDER BY column1, column2 DESC;</u></c><br/>
        /// <c>SELECT column1, column2 AS col2 <u>ORDER BY SUM(column1) ASC, col2;</u></c><br/>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex RegexOrderBy { get; } = new Regex(
            @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>ORDER BY</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>This expression performs a simple match for the <c>ORDER BY</c> keyword in a SQL statement, followed by a space.
        /// Everything after is excluded.</para>
        /// <c>SELECT * FROM tbl <u>ORDER BY</u> column1, column2 DESC;</c><br/>
        /// <c>SELECT column1, column2 AS col2 <u>ORDER BY</u> SUM(column1) ASC, col2;</c>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex SimpleRegexOrderBy { get; } = new Regex(@"\bORDER\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>GROUP BY</c> clause.
        /// </summary>
        /// <remarks>
        /// <para>Matches the <c>GROUP BY</c> clause, followed by one or more column names along with any aggregate functions or
        /// expressions. <c>AS</c> alias declarations are excluded from the match.</para>
        /// <c>SELECT * FROM tbl <u>GROUP BY column1, column2</u>;</c><br/>
        /// <c>SELECT column1, column2 <u>GROUP BY SUM(column1), column2</u>;</c><br/>
        /// <c>SELECT column1 AS col1 FROM tbl <u>GROUP BY col1</u> ORDER BY col1;</c>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex RegexGroupBy { get; } = new Regex(
            @"\bGROUP\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>GROUP BY</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>This expression performs a simple match for the <c>GROUP BY</c> keyword in a SQL statement, followed by a space.
        /// Everything after is excluded.</para>
        /// <c>SELECT * FROM tbl <u>GROUP BY</u> column1, column2;</c><br/>
        /// <c>SELECT column1, COUNT(column2) AS count_col2 FROM tbl <u>GROUP BY</u> column1 ORDER BY column1;</c>
        /// </remarks>
        [Obsolete("No longer required internally. Will be removed in future version")]
        public Regex SimpleRegexGroupBy { get; } = new Regex(@"\bGROUP\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the singleton instance of the <see cref="PagingHelper"/> class.
        /// </summary>
        public static IPagingHelper Instance { get; private set; }

        static PagingHelper() => Instance = new PagingHelper();

        /// <inheritdoc/>
        public bool SplitSQL(string sql, out SQLParts parts)
        {
            // TODO: add String.IsNullOrWhiteSpace() check for sql param
            // TODO: initialize parts in this method
            parts.Sql = sql;
            parts.SqlSelectRemoved = null;
            parts.SqlCount = sql;
            parts.SqlOrderBy = null;
            parts.SqlCte = null;
            parts.IsDistinct = false;

            var analysis = SqlClauseScanner.Analyze(sql);
            if (!analysis.Success)
                return false;

            parts.SqlCte = analysis.Cte;
            parts.IsDistinct = analysis.IsDistinct;

            // The generated statements are rebuilt from the outermost select list onwards, so that the text in front of it
            // (whitespace, comments, the terminator of an empty statement) and the statement terminator itself are not carried
            // into them. The common table expression list that the statement declares, if any, is carried into them, because
            // the outermost statement may refer to the expressions that it declares.
            var cte = parts.SqlCte ?? string.Empty;

            // The ORDER BY clause, the row limiting clauses that may follow it and the statement terminator are not part of
            // the statements used to select the page of records or to count the total number of records.
            var hasOrderBy = analysis.OrderByEnd > analysis.OrderByIndex && analysis.OrderByIndex > analysis.SelectListStart;
            var bodyEnd = hasOrderBy ? analysis.OrderByIndex : analysis.StatementEnd;

            parts.SqlSelectRemoved = sql.Substring(analysis.SelectListStart, bodyEnd - analysis.SelectListStart).TrimEnd();

            if (hasOrderBy)
            {
                // The clause is the ordering expression list only, so that it can safely be re-used in a
                // "ROW_NUMBER() OVER (...)" expression.
                parts.SqlOrderBy = sql.Substring(analysis.OrderByIndex, analysis.OrderByEnd - analysis.OrderByIndex).Trim();
            }

            // Replace the column list with COUNT(*). The statement is wrapped when the rows it selects are not one to one
            // with the rows of the tables it selects from.
            if (analysis.IsDistinct || analysis.GroupByIndex >= 0)
            {
                parts.SqlCount = $"{cte}SELECT COUNT(*) FROM (SELECT {parts.SqlSelectRemoved}) countAlias";
            }
            else
            {
                var fromOnwards = sql.Substring(analysis.SelectListEnd, bodyEnd - analysis.SelectListEnd).TrimEnd();
                parts.SqlCount = $"{cte}SELECT COUNT(*) {fromOnwards}";
            }

            return true;
        }

    }
}
