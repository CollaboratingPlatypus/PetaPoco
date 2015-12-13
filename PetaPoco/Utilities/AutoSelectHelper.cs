// <copyright file="AutoSelectHelper.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System.Linq;
using System.Text.RegularExpressions;
using PetaPoco.Core;

namespace PetaPoco.Internal
{
    internal static class AutoSelectHelper
    {
        private static Regex rxSelect = new Regex(@"\A\s*(SELECT|EXECUTE|CALL)\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static Regex rxFrom = new Regex(@"\A\s*FROM\s",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static string AddSelectClause<T>(DatabaseType DatabaseType, string sql)
        {
            if (sql.StartsWith(";"))
                return sql.Substring(1);

            if (!rxSelect.IsMatch(sql))
            {
                var pd = PocoData.ForType(typeof(T));
                var tableName = DatabaseType.EscapeTableName(pd.TableInfo.TableName);
                string cols = pd.Columns.Count != 0
                    ? string.Join(", ", (from c in pd.QueryColumns select tableName + "." + DatabaseType.EscapeSqlIdentifier(c)).ToArray())
                    : "NULL";
                if (!rxFrom.IsMatch(sql))
                    sql = string.Format("SELECT {0} FROM {1} {2}", cols, tableName, sql);
                else
                    sql = string.Format("SELECT {0} {1}", cols, sql);
            }
            return sql;
        }
    }
}