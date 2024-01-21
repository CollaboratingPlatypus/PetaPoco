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
    /// Provides an abstract base class for database providers. This class implements common functionality and provides default behavior for
    /// specialized database providers.
    /// </summary>
    /// <remarks>
    /// This class includes methods for database-specific operations like parameter handling, SQL escaping, and paging, among others.
    /// Derived classes should override these methods to implement behavior specific to the database they target.
    /// </remarks>
    public abstract class DatabaseProvider : IProvider
    {
        private static readonly ConcurrentDictionary<string, IProvider> customProviders = new ConcurrentDictionary<string, IProvider>();

        /// <inheritdoc/>
        public abstract DbProviderFactory GetFactory();

        /// <inheritdoc/>
        public virtual bool HasNativeGuidSupport => false;

        /// <inheritdoc/>
        public virtual IPagingHelper PagingUtility => PagingHelper.Instance;

        /// <inheritdoc/>
        public virtual string EscapeTableName(string tableName)
            => tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);

        /// <inheritdoc/>
        public virtual string EscapeSqlIdentifier(string sqlIdentifier) => $"[{sqlIdentifier}]";

        /// <inheritdoc/>
        public virtual string GetParameterPrefix(string connectionString) => "@";

        /// <inheritdoc/>
        public virtual object MapParameterValue(object value) => value is bool b ? b ? 1 : 0 : value;

        /// <inheritdoc/>
        public virtual void PreExecute(IDbCommand cmd)
        {
        }

        /// <inheritdoc/>
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nLIMIT @{args.Length} OFFSET @{args.Length + 1}";
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
        }

        /// <inheritdoc/>
        public virtual string GetExistsSql() => "SELECT COUNT(*) FROM {0} WHERE {1}";

        /// <inheritdoc/>
        public virtual string GetAutoIncrementExpression(TableInfo tableInfo) => null;

        /// <inheritdoc/>
        public virtual string GetInsertOutputClause(string primaryKeyName) => string.Empty;

        /// <inheritdoc/>
        public virtual object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(db, cmd);
        }

#if ASYNC
        /// <inheritdoc/>
        public virtual Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelperAsync(cancellationToken, db, cmd);
        }
#endif

        /// <summary>
        /// Returns the provider factory from one or more specified assembly qualified names.
        /// </summary>
        /// <param name="assemblyQualifiedNames">One or more assembly qualified names of the provider factory.</param>
        /// <returns>The provider factory.</returns>
        /// <exception cref="ArgumentException">None of the <paramref name="assemblyQualifiedNames"/> match a type.</exception>
        protected DbProviderFactory GetFactory(params string[] assemblyQualifiedNames)
        {
            Type ft = null;
            foreach (var assemblyName in assemblyQualifiedNames)
            {
                ft = Type.GetType(assemblyName);
                if (ft != null)
                    break;
            }

            // TODO: Possible incorrect type name used when throwing ArgumentException?
            if (ft == null)
                throw new ArgumentException($"Could not load the {GetType().Name} DbProviderFactory.");

            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }

        /// <summary>
        /// Registers a custom IProvider with a string that will match the beginning of the name of the provider, DbConnection, or
        /// DbProviderFactory.
        /// </summary>
        /// <typeparam name="T">The type of IProvider to be registered.</typeparam>
        /// <param name="initialString">The string to be matched against the beginning of the provider name.</param>
        /// <exception cref="ArgumentException"><paramref name="initialString"/> is null, empty, or consists of only white
        /// space.</exception>
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

        internal static void ClearCustomProviders() => customProviders.Clear();

        /// <summary>
        /// Instantiates a suitable IProvider instance based on the specified provider's type.
        /// </summary>
        /// <param name="providerType">The type of provider to be registered.</param>
        /// <param name="allowDefault">Specifies whether to allow the default <see cref="SqlServerDatabaseProvider"/> to be returned if no
        /// matching provider is found.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The resolved database provider.</returns>
        /// <exception cref="ArgumentException">The <paramref name="providerType"/> name cannot be matched to a provider.</exception>
        internal static IProvider Resolve(Type providerType, bool allowDefault, string connectionString)
        {
            var typeName = providerType.Name;

            // Try using type name first (more reliable)
            var custom = GetCustomProvider(typeName);
            if (custom != null)
                return custom;

            if (providerType.Namespace != null)
            {
                if (typeName.Equals("SqlConnection") && providerType.Namespace.StartsWith("Microsoft.Data") ||
                    providerType.Namespace.StartsWith("Microsoft.Data") && typeName.Equals("SqlClientFactory"))
                    return Singleton<SqlServerMsDataDatabaseProvider>.Instance;
            }

            if (typeName.Equals("SqlConnection") || typeName.Equals("SqlClientFactory"))
                return Singleton<SqlServerDatabaseProvider>.Instance;

            if (typeName.StartsWith("MySqlConnector"))
                return Singleton<MySqlConnectorDatabaseProvider>.Instance;

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
                throw new ArgumentException($"Could not match `{providerType.FullName}` to a provider.", nameof(providerType));

            // Assume SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }

        /// <summary>
        /// Instantiates a suitable IProvider instance based on the specified provider name.
        /// </summary>
        /// <param name="providerName">The provider name.</param>
        /// <param name="allowDefault">Specifies whether to allow the default <see cref="SqlServerDatabaseProvider"/> to be returned if no
        /// matching provider is found.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The resolved database provider.</returns>
        /// <exception cref="ArgumentException">The <paramref name="providerName"/> name cannot be matched to a provider.</exception>
        internal static IProvider Resolve(string providerName, bool allowDefault, string connectionString)
        {
            // Try again with provider name
            var custom = GetCustomProvider(providerName);
            if (custom != null)
                return custom;

            if (providerName.IndexOf("Microsoft.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerMsDataDatabaseProvider>.Instance;

            if (providerName.IndexOf("SqlServer", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerDatabaseProvider>.Instance;

            if (providerName.IndexOf("MySqlConnector", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MySqlConnectorDatabaseProvider>.Instance;

            if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MySqlDatabaseProvider>.Instance;

            if (providerName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MariaDbDatabaseProvider>.Instance;

            if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerCEDatabaseProviders>.Instance;

            if (providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
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
        /// Unwraps the specified wrapped provider factory/>.
        /// </summary>
        /// <param name="factory">The database provider factory to unwrap.</param>
        /// <returns>The unwrapped factory, or the original factory if no wrapping occurred.</returns>
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

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        protected void ExecuteNonQueryHelper(Database db, IDbCommand cmd) => db.ExecuteNonQueryHelper(cmd);

        /// <summary>
        /// Executes a query command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        protected object ExecuteScalarHelper(Database db, IDbCommand cmd) => db.ExecuteScalarHelper(cmd);

#if ASYNC
        /// <summary>
        /// Asynchronously executes a non-query command.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        protected Task ExecuteNonQueryHelperAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd)
            => db.ExecuteNonQueryHelperAsync(cancellationToken, cmd);

        /// <summary>
        /// Asynchronously executes a query command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="db">The database instance that will execute the SQL command.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the first column of the first row in the result set.
        /// </returns>
        protected Task<object> ExecuteScalarHelperAsync(CancellationToken cancellationToken, Database db, IDbCommand cmd)
            => db.ExecuteScalarHelperAsync(cancellationToken, cmd);
#endif
    }
}
