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
	// TODO: It irks the ¢®@₱ out of me that the Firebird DBProvider class name is the only one that improperly appends "Db" after the provider name.
    /// <summary>
    /// The FirebirdDbDatabaseProvider class provides an implementation of the <see cref="DatabaseProvider"/> class specific to the Firebird database.
    /// </summary>
    public class FirebirdDbDatabaseProvider : DatabaseProvider
    {
        /// <inheritdoc/>
        public override DbProviderFactory GetFactory()
            => GetFactory("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory, FirebirdSql.Data.FirebirdClient");

        /// <inheritdoc/>
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nROWS @{args.Length} TO @{args.Length + 1}";
            args = args.Concat(new object[] { skip + 1, skip + take }).ToArray();
            return sql;
        }

        /// <inheritdoc/>
        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            PrepareInsert(cmd, primaryKeyName);
            return ExecuteScalarHelper(db, cmd);
        }

#if ASYNC
        /// <inheritdoc/>
        public override Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            PrepareInsert(cmd, primaryKeyName);
            return ExecuteScalarHelperAsync(cancellationToken, db, cmd);
        }
#endif

        private void PrepareInsert(IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText = cmd.CommandText.TrimEnd();

            if (cmd.CommandText.EndsWith(";"))
                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1);

            cmd.CommandText += " RETURNING " + EscapeSqlIdentifier(primaryKeyName) + ";";
        }

        /// <inheritdoc/>
        public override string EscapeSqlIdentifier(string sqlIdentifier) => $"\"{sqlIdentifier}\"";
    }
}
