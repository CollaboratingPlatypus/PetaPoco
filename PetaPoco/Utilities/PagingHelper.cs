using System.Text.RegularExpressions;

namespace PetaPoco.Utilities
{
    public class PagingHelper : IPagingHelper
    {
        public Regex RegexColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex RegexDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex RegexOrderBy =
            new Regex(
                @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
                RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex SimpleRegexOrderBy = new Regex(@"\bORDER\s+BY\s+",
            RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex RegexGroupBy = new Regex(@"\bGROUP\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\[\]`""\w\(\)\.])+?)*",
                                              RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public Regex SimpleRegexGroupBy = new Regex(@"\bGROUP\s+BY\s+",
                                                    RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);


        public static IPagingHelper Instance { get; private set; }

        static PagingHelper()
        {
            Instance = new PagingHelper();
        }

        /// <summary>
        ///     Splits the given <paramref name="sql" /> into <paramref name="parts" />;
        /// </summary>
        /// <param name="sql">The SQL to split.</param>
        /// <param name="parts">The SQL parts.</param>
        /// <returns><c>True</c> if the SQL could be split; else, <c>False</c>.</returns>
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