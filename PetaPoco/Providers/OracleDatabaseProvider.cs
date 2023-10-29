using System;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using PetaPoco.Core;
using PetaPoco.Utilities;
using System.Linq;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif

namespace PetaPoco.Providers
{
    /// <summary>
    /// Provides an implementation of <see cref="DatabaseProvider"/> for Oracle databases using the managed Oracle Data Provider.
    /// </summary>
    /// <remarks>
    /// This provider uses the "Oracle.ManagedDataAccess.Client" ADO.NET driver for data access.
    /// </remarks>
    public class OracleDatabaseProvider : DatabaseProvider
    {
        //An ordinary identifier must begin with a letter and contain only letters, underscore characters (_), and digits.
        //The permitted letters and digits include all Unicode letters and digits.
        //A delimited identifier is surrounded by double quotation marks and can contain any characters within the double quotation marks.
        //Maximum two identifiers can be joined, separated by a dot (.)
        private static readonly Regex _ordinaryIdentifierRegex = new Regex(@"^[\p{L}]+[\p{L}\d_]*(?:\.[\p{L}]+[\p{L}\d_]*)?$", RegexOptions.Compiled);
        private static readonly Regex _delimitedIdentifierRegex = new Regex(@"^""[^""]*(?:""\.""[^""]*)?""$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public override string GetParameterPrefix(string connectionString) => ":";

        /// <inheritdoc/>
        public override void PreExecute(IDbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName")?.SetValue(cmd, true, null);
            cmd.GetType().GetProperty("InitialLONGFetchSize")?.SetValue(cmd, -1, null);

            //By default statements are cached, so if the database is modified between two reads (same statement),
            //the last one will still access the old statement's definition.
            //Setting this property to false prevents caching of statements completely.
            //NOTE: Using "Statement Cache Purge=true; Statement Cache Size=0" in the ConnectionString is not a guarantee
            cmd.GetType().GetProperty("AddToStatementCache")?.SetValue(cmd, false, null);
        }

        /// <inheritdoc/>
        /// <exception cref="Exception">A paged query does not alias '*'</exception>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.SqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            //Supported by Oracle v12c and above only
            var sql = $"{parts.Sql}\nOFFSET @{args.Length} ROWS FETCH NEXT @{args.Length + 1} ROWS ONLY";
            args = args.Concat(new object[] { skip, take }).ToArray();
            return sql;

            //Older versions of Oracle
            ////Similar to SqlServerProvider with the exception of SELECT NULL FROM DUAL vs SELECT NULL
            //var helper = (PagingHelper)PagingUtility;
            //// when the query does not contain an "order by", it is very slow
            //if (helper.SimpleRegexOrderBy.IsMatch(parts.SqlSelectRemoved))
            //{
            //    var m = helper.SimpleRegexOrderBy.Match(parts.SqlSelectRemoved);
            //    if (m.Success)
            //    {
            //        var g = m.Groups[0];
            //        parts.SqlSelectRemoved = parts.SqlSelectRemoved.Substring(0, g.Index);
            //    }
            //}

            //if (helper.RegexDistinct.IsMatch(parts.SqlSelectRemoved))
            //    parts.SqlSelectRemoved = "peta_inner.* FROM (SELECT " + parts.SqlSelectRemoved + ") peta_inner";

            //var sqlPage =
            //    $"SELECT * FROM (SELECT ROW_NUMBER() OVER ({parts.SqlOrderBy ?? "ORDER BY (SELECT NULL FROM DUAL)"}) peta_rn, {parts.SqlSelectRemoved}) peta_paged WHERE peta_rn > @{args.Length} AND peta_rn <= @{args.Length + 1}";
            //args = args.Concat(new object[] { skip, skip + take }).ToArray();
            //return sqlPage;
        }

        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
        {
            // "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess" is for Oracle.ManagedDataAccess.dll
            // "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess" is for Oracle.DataAccess.dll
            return GetFactory("Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess");
        }

        /// <inheritdoc/>
        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            //If already quoted, leave as-is
            if (_delimitedIdentifierRegex.IsMatch(sqlIdentifier)) return sqlIdentifier;

            //If using ordinary identifiers and no quotes required, leave as-is (could also uppercase)
            if (UseOrdinaryIdentifiers && _ordinaryIdentifierRegex.IsMatch(sqlIdentifier)) return sqlIdentifier; //.ToUpperInvariant();

            //If using delimited identifiers or quotes required, wrap in quotes, but don't allow use of double quotes in identifier
            return "\"" + sqlIdentifier.Replace("\"", "").Replace(".", "\".\"") + "\"";
        }

        /// <inheritdoc/>
        public override string GetAutoIncrementExpression(TableInfo ti) => !string.IsNullOrEmpty(ti.SequenceName) ? $"{ti.SequenceName}.nextval" : null;

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                var param = PrepareInsert(cmd, primaryKeyName);
                ExecuteNonQueryHelper(db, cmd);
                return param.Value;
            }

            ExecuteNonQueryHelper(db, cmd);
            return -1;
        }

#if ASYNC
        /// <inheritdoc/>
        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                var param = PrepareInsert(cmd, primaryKeyName);
                await ExecuteNonQueryHelperAsync(cancellationToken, db, cmd).ConfigureAwait(false);
                return param.Value;
            }

            await ExecuteNonQueryHelperAsync(cancellationToken, db, cmd).ConfigureAwait(false);
            return -1;
        }
#endif

        private IDbDataParameter PrepareInsert(IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += $" returning {EscapeSqlIdentifier(primaryKeyName)} into :newid";
            var param = cmd.CreateParameter();
            param.ParameterName = ":newid";
            param.Value = DBNull.Value;
            param.Direction = ParameterDirection.ReturnValue;
            param.DbType = DbType.Int64;
            cmd.Parameters.Add(param);
            return param;
        }
    }
}
