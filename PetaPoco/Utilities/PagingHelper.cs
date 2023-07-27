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
        /// <para>Beginning at the start of the string, this expression matches the keyword <c>SELECT</c> (inclusive), followed by one or more spaces, capturing everything in front of the word <c>FROM</c> (exclusive), accounting for special handling of nested parentheses, aggregate functions, etc.</para>
        /// <c><u>SELECT column1, column2</u> FROM tbl;</c><br/>
        /// <c><u>SELECT SUM(column1) AS sum_col1, column2</u> FROM tbl;</c>
        /// </remarks>
        public Regex RegexColumns { get; } = new Regex(
            @"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>DISTINCT</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>Starting at the beginning of the string, this expression performs a simple match for the <c>DISTINCT</c> keyword in a SQL statement, followed by a space. Everything after is excluded.</para>
        /// <c>SELECT <u>DISTINCT</u> * FROM tbl;</c><br/>
        /// <c>SELECT <u>DISTINCT</u> column1 FROM tbl;</c>
        /// </remarks>
        public Regex RegexDistinct { get; } = new Regex(@"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>ORDER BY</c> clause.
        /// </summary>
        /// <remarks>
        /// <para>This expression matches the <c>ORDER BY</c> clause, followed by one or more column names as well as the accompanying optional sort order modifier keywords, <c>ASC</c> and <c>DESC</c>, for each (inclusive). <c>AS</c> alias declarations are excluded from the match.</para>
        /// <c>SELECT * FROM tbl <u>ORDER BY column1, column2 DESC;</u></c><br/>
        /// <c>SELECT column1, column2 AS col2 <u>ORDER BY SUM(column1) ASC, col2;</u></c><br/>
        /// </remarks>
        public Regex RegexOrderBy { get; } = new Regex(
            @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>ORDER BY</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>This expression performs a simple match for the <c>ORDER BY</c> keyword in a SQL statement, followed by a space. Everything after is excluded.</para>
        /// <c>SELECT * FROM tbl <u>ORDER BY</u> column1, column2 DESC;</c><br/>
        /// <c>SELECT column1, column2 AS col2 <u>ORDER BY</u> SUM(column1) ASC, col2;</c>
        /// </remarks>
        public Regex SimpleRegexOrderBy { get; } = new Regex(@"\bORDER\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>GROUP BY</c> clause.
        /// </summary>
        /// <remarks>
        /// <para>Matches the <c>GROUP BY</c> clause, followed by one or more column names along with any aggregate functions or expressions. <c>AS</c> alias declarations are excluded from the match.</para>
        /// <c>SELECT * FROM tbl <u>GROUP BY column1, column2</u>;</c><br/>
        /// <c>SELECT column1, column2 <u>GROUP BY SUM(column1), column2</u>;</c><br/>
        /// <c>SELECT column1 AS col1 FROM tbl <u>GROUP BY col1</u> ORDER BY col1;</c>
        /// </remarks>
        public Regex RegexGroupBy { get; } = new Regex(
            @"\bGROUP\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the <c>GROUP BY</c> keyword.
        /// </summary>
        /// <remarks>
        /// <para>This expression performs a simple match for the <c>GROUP BY</c> keyword in a SQL statement, followed by a space. Everything after is excluded.</para>
        /// <c>SELECT * FROM tbl <u>GROUP BY</u> column1, column2;</c><br/>
        /// <c>SELECT column1, COUNT(column2) AS count_col2 FROM tbl <u>GROUP BY</u> column1 ORDER BY column1;</c>
        /// </remarks>
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

            // Extract the columns from "SELECT <whatever> FROM"
            var columnsMatch = RegexColumns.Match(sql);
            if (!columnsMatch.Success)
                return false;

            // Look for the last "ORDER BY <whatever>" clause not part of a ROW_NUMBER expression
            var orderByMatch = RegexOrderBy.Match(sql);
            if (orderByMatch.Success)
            {
                parts.SqlOrderBy = orderByMatch.Value;
                parts.SqlCount = sql.Replace(orderByMatch.Value, string.Empty);
            }

            // Save column list and replace with COUNT(*)
            var columnsGroup = columnsMatch.Groups[1];
            parts.SqlSelectRemoved = sql.Substring(columnsGroup.Index);

            if (RegexDistinct.IsMatch(parts.SqlSelectRemoved) || SimpleRegexGroupBy.IsMatch(parts.SqlSelectRemoved))
            {
                parts.SqlCount = sql.Substring(0, columnsGroup.Index) + "COUNT(*) FROM (" + parts.SqlCount + ") countAlias";
            }
            else
            {
                parts.SqlCount = sql.Substring(0, columnsGroup.Index) + "COUNT(*) " + parts.SqlCount.Substring(columnsGroup.Index + columnsGroup.Length);
            }

            return true;
        }
    }
}
