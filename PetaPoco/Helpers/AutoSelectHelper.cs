using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PetaPoco
{
	static class AutoSelectHelper
	{
		public static string AddSelectClause<T>(DatabaseType DatabaseType, string sql)
		{
			if (sql.StartsWith(";"))
				return sql.Substring(1);

			if (!rxSelect.IsMatch(sql))
			{
				var pd = PocoData.ForType(typeof(T));
				var tableName = DatabaseType.EscapeTableName(pd.TableInfo.TableName);
				string cols = string.Join(", ", (from c in pd.QueryColumns select tableName + "." + DatabaseType.EscapeSqlIdentifier(c)).ToArray());
				if (!rxFrom.IsMatch(sql))
					sql = string.Format("SELECT {0} FROM {1} {2}", cols, tableName, sql);
				else
					sql = string.Format("SELECT {0} {1}", cols, sql);
			}
			return sql;
		}

		static Regex rxSelect = new Regex(@"\A\s*(SELECT|EXECUTE|CALL)\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
		static Regex rxFrom = new Regex(@"\A\s*FROM\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
	}
}
