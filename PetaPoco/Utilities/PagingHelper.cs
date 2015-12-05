// <copyright file="PagingHelper.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System.Text.RegularExpressions;

namespace PetaPoco.Internal
{
    internal static class PagingHelper
    {
        public static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex rxOrderBy =
            new Regex(
                @"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
                RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public static Regex rxDistinct = new Regex(@"\ADISTINCT\s",
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        public static bool SplitSQL(string sql, out SQLParts parts)
        {
            parts.sql = sql;
            parts.sqlSelectRemoved = null;
            parts.sqlCount = null;
            parts.sqlOrderBy = null;

            // Extract the columns from "SELECT <whatever> FROM"
            var m = rxColumns.Match(sql);
            if (!m.Success)
                return false;

            // Save column list and replace with COUNT(*)
            Group g = m.Groups[1];
            parts.sqlSelectRemoved = sql.Substring(g.Index);

            if (rxDistinct.IsMatch(parts.sqlSelectRemoved))
                parts.sqlCount = sql.Substring(0, g.Index) + "COUNT(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
            else
                parts.sqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);

            // Look for the last "ORDER BY <whatever>" clause not part of a ROW_NUMBER expression
            m = rxOrderBy.Match(parts.sqlCount);
            if (!m.Success)
            {
                parts.sqlOrderBy = null;
            }
            else
            {
                g = m.Groups[0];
                parts.sqlOrderBy = g.ToString();
                parts.sqlCount = parts.sqlCount.Substring(0, g.Index) + parts.sqlCount.Substring(g.Index + g.Length);
            }

            return true;
        }

        public struct SQLParts
        {
            public string sql;
            public string sqlCount;
            public string sqlSelectRemoved;
            public string sqlOrderBy;
        }
    }
}