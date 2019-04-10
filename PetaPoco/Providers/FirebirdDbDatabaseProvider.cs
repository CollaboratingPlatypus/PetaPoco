using System.Data;
using System.Data.Common;
using System.Linq;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
            => GetFactory("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient");

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nROWS @{args.Length} TO @{args.Length + 1}";
            args = args.Concat(new object[] { skip + 1, skip + take }).ToArray();
            return sql;
        }

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            PrepareInsert(cmd, primaryKeyName);
            return ExecuteScalarHelper(database, cmd);
        }

#if ASYNC
        public override Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database database, IDbCommand cmd, string primaryKeyName)
        {
            PrepareInsert(cmd, primaryKeyName);
            return ExecuteScalarHelperAsync(cancellationToken, database, cmd);
        }
#endif

        private void PrepareInsert(IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText = cmd.CommandText.TrimEnd();

            if (cmd.CommandText.EndsWith(";"))
                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);

            cmd.CommandText += " RETURNING " + EscapeSqlIdentifier(primaryKeyName) + ";";
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
            => $"\"{sqlIdentifier}\"";
    }
}