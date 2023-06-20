using System.Text.RegularExpressions;

namespace PetaPoco.Utilities
{
    /// <summary>
    /// Provides utility methods for splitting SQL queries into parts and handling paging.
    /// </summary>
    public class PagingHelper : IPagingHelper
    {
        /// <summary>
        /// Gets the regular expression used for matching the columns in a SELECT statement.
        /// </summary>
        public Regex RegexColumns { get; } = new Regex(
            @"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the DISTINCT keyword.
        /// </summary>
        public Regex RegexDistinct { get; } = new Regex(
            @"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the ORDER BY clause.
        /// </summary>
        public Regex RegexOrderBy { get; } = new Regex(
            @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the simple ORDER BY clause.
        /// </summary>
        public Regex SimpleRegexOrderBy { get; } = new Regex(
            @"\bORDER\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the GROUP BY clause.
        /// </summary>
        public Regex RegexGroupBy { get; } = new Regex(
            @"\bGROUP\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?)*",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the regular expression used for matching the simple GROUP BY clause.
        /// </summary>
        public Regex SimpleRegexGroupBy { get; } = new Regex(
            @"\bGROUP\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Gets the singleton instance of the <see cref="PagingHelper"/> class.
        /// </summary>
        public static IPagingHelper Instance { get; private set; }

        static PagingHelper() => Instance = new PagingHelper();

        /// <inheritdoc/>
        public bool SplitSQL(string sql, out SQLParts parts)
        {
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
