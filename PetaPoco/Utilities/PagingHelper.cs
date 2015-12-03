// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace PetaPoco.Internal
{
	internal static class PagingHelper
	{
		public struct SQLParts
		{
			public string sql;
			public string sqlCount;
			public string sqlSelectRemoved;
			public string sqlOrderBy;
		}

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

		public static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
     	public static Regex rxDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
	}
}
