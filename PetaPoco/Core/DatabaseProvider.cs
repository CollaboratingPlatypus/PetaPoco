using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif
using PetaPoco.Internal;
using PetaPoco.Providers;
using PetaPoco.Utilities;

namespace PetaPoco.Core
{
    /// <summary>
    ///     Base class for DatabaseType handlers - provides default/common handling for different database engines
    /// </summary>
    public abstract class DatabaseProvider : IProvider
    {
        private static readonly ConcurrentDictionary<string, IProvider> customProviders = new ConcurrentDictionary<string, IProvider>();

        /// <inheritdoc />
        public abstract DbProviderFactory GetFactory();

        /// <inheritdoc />
        public virtual bool HasNativeGuidSupport => false;

        /// <inheritdoc />
        public virtual IPagingHelper PagingUtility => PagingHelper.Instance;

        /// <inheritdoc />
        public virtual string EscapeTableName(string tableName)
            => tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);

        /// <inheritdoc />
        public virtual string EscapeSqlIdentifier(string sqlIdentifier)
            => $"[{sqlIdentifier}]";

        /// <inheritdoc />
        public virtual string GetParameterPrefix(string connectionString)
            => "@";

        /// <inheritdoc />
        public virtual object MapParameterValue(object value)
        {
            if (value is bool b)
                return b ? 1 : 0;

            return value;
        }

        /// <inheritdoc />
        public virtual void PreExecute(IDbCommand cmd)
        {
        }

        /// <inheritdoc />
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nLIMIT @{args.Length} OFFSET @{args.Length + 1}";
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
        }

        /// <inheritdoc />
        public virtual string GetExistsSql()
            => "SELECT COUNT(*) FROM {0} WHERE {1}";

        /// <inheritdoc />
        public virtual string GetAutoIncrementExpression(TableInfo tableInfo)
            => null;

        /// <inheritdoc />
        public virtual string GetInsertOutputClause(string primaryKeyName)
            => string.Empty;

        /// <inheritdoc />
        public virtual object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(database, cmd);
        }

#if ASYNC
        public virtual Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database database, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelperAsync(cancellationToken, database, cmd);
        }
#endif

        /// <summary>
        ///     Returns the DbProviderFactory.
        /// </summary>
        /// <param name="assemblyQualifiedNames">The assembly qualified name of the provider factory.</param>
        /// <returns>The db provider factory.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="assemblyQualifiedNames" /> does not match a type.</exception>
        protected DbProviderFactory GetFactory(params string[] assemblyQualifiedNames)
        {
            Type ft = null;
            foreach (var assemblyName in assemblyQualifiedNames)
            {
                ft = Type.GetType(assemblyName);
                if (ft != null)
                    break;
            }

            if (ft == null)
                throw new ArgumentException($"Could not load the {GetType().Name} DbProviderFactory.");

            return (DbProviderFactory) ft.GetField("Instance").GetValue(null);
        }

        /// <summary>
        ///     Registers a custom IProvider with a string that will match the beginning of the name
        ///     of the provider, DbConnection, or DbProviderFactory.
        /// </summary>
        /// <typeparam name="T">Type of IProvider to be registered.</typeparam>
        /// <param name="initialString">String to be matched against the beginning of the provider name.</param>
        public static void RegisterCustomProvider<T>(string initialString) where T : IProvider, new()
        {
            if (String.IsNullOrWhiteSpace(initialString))
                throw new ArgumentException("Initial string must not be null or empty", nameof(initialString));

            customProviders[initialString] = Singleton<T>.Instance;
        }

        private static IProvider GetCustomProvider(string name)
        {
            IProvider provider;
            foreach (var initialString in customProviders.Keys)
                if (name.IndexOf(initialString, StringComparison.InvariantCultureIgnoreCase) == 0 && customProviders.TryGetValue(initialString, out provider))
                    return provider;

            return null;
        }

        internal static void ClearCustomProviders()
            => customProviders.Clear();

        /// <summary>
        ///     Look at the type and provider name being used and instantiate a suitable IProvider instance.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <param name="allowDefault">
        ///     A flag that when set allows the default <see cref="SqlServerDatabaseProvider" /> to be
        ///     returned if not match is found.
        /// </param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The database provider.</returns>
        internal static IProvider Resolve(Type type, bool allowDefault, string connectionString)
        {
            var typeName = type.Name;

            // Try using type name first (more reliable)
            var custom = GetCustomProvider(typeName);
            if (custom != null)
                return custom;

            if (type.Namespace != null)
            {
                if (typeName.Equals("SqlConnection") && type.Namespace.StartsWith("Microsoft.Data") ||
                    type.Namespace.StartsWith("Microsoft.Data") && typeName.Equals("SqlClientFactory"))
                    return Singleton<SqlSererSqlClientDatabaseProvider>.Instance;
            }

            if (typeName.Equals("SqlConnection") || typeName.Equals("SqlClientFactory"))
                return Singleton<SqlServerDatabaseProvider>.Instance;

            if (typeName.StartsWith("MySql"))
                return Singleton<MySqlDatabaseProvider>.Instance;

            if (typeName.StartsWith("MariaDb"))
                return Singleton<MariaDbDatabaseProvider>.Instance;

            if (typeName.StartsWith("SqlCe"))
                return Singleton<SqlServerCEDatabaseProviders>.Instance;

            if (typeName.StartsWith("Npgsql") || typeName.StartsWith("PgSql"))
                return Singleton<PostgreSQLDatabaseProvider>.Instance;

            if (typeName.StartsWith("Oracle"))
                return Singleton<OracleDatabaseProvider>.Instance;

            if (typeName.StartsWith("SQLite") || typeName.StartsWith("Sqlite"))
                return Singleton<SQLiteDatabaseProvider>.Instance;

            if (typeName.StartsWith("FbConnection") || typeName.EndsWith("FirebirdClientFactory"))
                return Singleton<FirebirdDbDatabaseProvider>.Instance;

            if (typeName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                (connectionString.IndexOf("Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                 connectionString.IndexOf("ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0))
            {
                return Singleton<MsAccessDbDatabaseProvider>.Instance;
            }

            if (!allowDefault)
                throw new ArgumentException($"Could not match `{type.FullName}` to a provider.", nameof(type));

            // Assume SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }

        /// <summary>
        ///     Look at the type and provider name being used and instantiate a suitable IProvider instance.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <param name="allowDefault">
        ///     A flag that when set allows the default <see cref="SqlServerDatabaseProvider" /> to be
        ///     returned if not match is found.
        /// </param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The database type.</returns>
        internal static IProvider Resolve(string providerName, bool allowDefault, string connectionString)
        {
            // Try again with provider name
            var custom = GetCustomProvider(providerName);
            if (custom != null)
                return custom;

            if (providerName.IndexOf("Microsoft.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlSererSqlClientDatabaseProvider>.Instance;

            if (providerName.IndexOf("SqlServer", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerDatabaseProvider>.Instance;

            if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MySqlDatabaseProvider>.Instance;

            if (providerName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MariaDbDatabaseProvider>.Instance;

            if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerCEDatabaseProviders>.Instance;

            if (providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0 || providerName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<PostgreSQLDatabaseProvider>.Instance;

            if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<OracleDatabaseProvider>.Instance;

            if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SQLiteDatabaseProvider>.Instance;

            if (providerName.IndexOf("Firebird", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("FbConnection", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<FirebirdDbDatabaseProvider>.Instance;

            if (providerName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0 &&
                (connectionString.IndexOf("Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                 connectionString.IndexOf("ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0))
            {
                return Singleton<MsAccessDbDatabaseProvider>.Instance;
            }



            if (!allowDefault)
                throw new ArgumentException($"Could not match `{providerName}` to a provider.", nameof(providerName));

            // Assume SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }

        /// <summary>
        ///     Unwraps a wrapped <see cref="DbProviderFactory" />.
        /// </summary>
        /// <param name="factory">The factory to unwrap.</param>
        /// <returns>The unwrapped factory or the original factory if no wrapping occurred.</returns>
        internal static DbProviderFactory Unwrap(DbProviderFactory factory)
        {
            if (!(factory is IServiceProvider sp))
                return factory;

            try
            {
                return !(sp.GetService(factory.GetType()) is DbProviderFactory unwrapped) ? factory : Unwrap(unwrapped);
            }
            catch (Exception)
            {
                return factory;
            }
        }

        protected void ExecuteNonQueryHelper(Database db, IDbCommand cmd) => db.ExecuteNonQueryHelper(cmd);

        protected object ExecuteScalarHelper(Database db, IDbCommand cmd) => db.ExecuteScalarHelper(cmd);

#if ASYNC
        protected Task ExecuteNonQueryHelperAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd)
            => db.ExecuteNonQueryHelperAsync(cancellationToken, cmd);

        protected Task<object> ExecuteScalarHelperAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd)
            => db.ExecuteScalarHelperAsync(cancellationToken, cmd);
#endif
    }
}