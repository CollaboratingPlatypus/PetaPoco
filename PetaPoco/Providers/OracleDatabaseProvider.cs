using System;
using System.Data;
using System.Data.Common;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;
using System.Threading;
using System.Threading.Tasks;


namespace PetaPoco.Providers
{
    public class OracleDatabaseProvider : DatabaseProvider
    {
        public override string GetParameterPrefix(string connectionString)
            => ":";

        public override void PreExecute(IDbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName")?.SetValue(cmd, true, null);
            cmd.GetType().GetProperty("InitialLONGFetchSize")?.SetValue(cmd, -1, null);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.SqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            // Same deal as SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance.BuildPageQuery(skip, take, parts, ref args);
        }

        public override DbProviderFactory GetFactory()
        {
            // "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess" is for Oracle.ManagedDataAccess.dll
            // "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess" is for Oracle.DataAccess.dll
            return GetFactory("Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess");
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
            => $"\"{sqlIdentifier.ToUpperInvariant()}\"";

        public override string GetAutoIncrementExpression(TableInfo ti)
            => !string.IsNullOrEmpty(ti.SequenceName) ? $"{ti.SequenceName}.nextval" : null;

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
    }
}