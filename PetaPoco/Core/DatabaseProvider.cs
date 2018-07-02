// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/06/28</date>

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        private static readonly ConcurrentDictionary<string, IProvider> _customProviders = new ConcurrentDictionary<string, IProvider>();

        /// <summary>
        ///     Gets the DbProviderFactory for this database provider.
        /// </summary>
        /// <returns>The provider factory.</returns>
        public abstract DbProviderFactory GetFactory();

        /// <summary>
        ///     Gets a flag for whether the DB has native support for GUID/UUID.
        /// </summary>
        public virtual bool HasNativeGuidSupport => false;

        /// <summary>
        ///     Gets the <seealso cref="IPagingHelper" /> this provider supplies.
        /// </summary>
        public virtual IPagingHelper PagingUtility => PagingHelper.Instance;

        /// <summary>
        ///     Escape a tablename into a suitable format for the associated database provider.
        /// </summary>
        /// <param name="tableName">
        ///     The name of the table (as specified by the client program, or as attributes on the associated
        ///     POCO class.
        /// </param>
        /// <returns>The escaped table name</returns>
        public virtual string EscapeTableName(string tableName) => tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);

        /// <summary>
        ///     Escape and arbitary SQL identifier into a format suitable for the associated database provider
        /// </summary>
        /// <param name="sqlIdentifier">The SQL identifier to be escaped</param>
        /// <returns>The escaped identifier</returns>
        public virtual string EscapeSqlIdentifier(string sqlIdentifier) => $"[{sqlIdentifier}]";

        /// <summary>
        ///     Returns the prefix used to delimit parameters in SQL query strings.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The providers character for prefixing a query parameter.</returns>
        public virtual string GetParameterPrefix(string connectionString) => "@";

        /// <summary>
        ///     Converts a supplied C# object value into a value suitable for passing to the database
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        public virtual object MapParameterValue(object value)
        {
            if (value is bool b)
                return b ? 1 : 0;

            return value;
        }

        /// <summary>
        ///     Called immediately before a command is executed, allowing for modification of the IDbCommand before it's passed to
        ///     the database provider
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void PreExecute(IDbCommand cmd)
        {

        }

        /// <summary>
        ///     Builds an SQL query suitable for performing page based queries to the database
        /// </summary>
        /// <param name="skip">The number of rows that should be skipped by the query</param>
        /// <param name="take">The number of rows that should be retruend by the query</param>
        /// <param name="parts">The original SQL query after being parsed into it's component parts</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL query</param>
        /// <returns>The final SQL query that should be executed.</returns>
        public virtual string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sql = $"{parts.Sql}\nLIMIT @{args.Length} OFFSET @{args.Length + 1}";
            args = args.Concat(new object[] { take, skip }).ToArray();
            return sql;
        }

        /// <summary>
        ///     Returns an SQL Statement that can check for the existence of a row in the database.
        /// </summary>
        /// <returns></returns>
        public virtual string GetExistsSql() => "SELECT COUNT(*) FROM {0} WHERE {1}";

        /// <summary>
        ///     Return an SQL expression that can be used to populate the primary key column of an auto-increment column.
        /// </summary>
        /// <param name="tableInfo">Table info describing the table</param>
        /// <returns>An SQL expressions</returns>
        /// <remarks>See the Oracle database type for an example of how this method is used.</remarks>
        public virtual string GetAutoIncrementExpression(TableInfo tableInfo) => null;

        /// <summary>
        ///     Returns an SQL expression that can be used to specify the return value of auto incremented columns.
        /// </summary>
        /// <param name="primaryKeyName">The primary key of the row being inserted.</param>
        /// <returns>An expression describing how to return the new primary key value</returns>
        /// <remarks>See the SQLServer database provider for an example of how this method is used.</remarks>
        public virtual string GetInsertOutputClause(string primaryKeyName)
        {
            return string.Empty;
        }

        /// <summary>
        ///     Performs an Insert operation
        /// </summary>
        /// <param name="database">The calling Database object</param>
        /// <param name="cmd">The insert command to be executed</param>
        /// <param name="primaryKeyName">The primary key of the table being inserted into</param>
        /// <returns>The ID of the newly inserted record</returns>
        public virtual object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(database, cmd);
        }

        /// <summary>
        ///     Returns the .net standard conforming DbProviderFactory.
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
                throw new ArgumentException("Could not load the " + GetType().Name + " DbProviderFactory.");

            return (DbProviderFactory) ft.GetField("Instance").GetValue(null);
        }

        /// <summary>
        ///     Registers a custom IProvider with a string that will the beginning of the name
        ///     of the provider, DbConnection, or DbProviderFactory.
        /// </summary>
        /// <typeparam name="T">Type of IProvider to be registered.</typeparam>
        /// <param name="initialString">String to be matched against the beginning of the provider name.</param>
        public static void RegisterCustomProvider<T>(string initialString) where T : IProvider, new()
        {
            if (String.IsNullOrWhiteSpace(initialString))
                throw new ArgumentException("Initial string must not be null or empty", "initialString");

            _customProviders[initialString] = Singleton<T>.Instance;
        }

        private static IProvider GetCustomProvider(string name)
        {
            IProvider provider;
            foreach (var initialString in _customProviders.Keys)
            {
                if (name.IndexOf(initialString, StringComparison.InvariantCultureIgnoreCase) == 0
                    && _customProviders.TryGetValue(initialString, out provider))
                {
                    return provider;
                }
            }

            return null;
        }

        internal static void ClearCustomProviders()
        {
            _customProviders.Clear();
        }

        /// <summary>
        ///     Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
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
            if (typeName.Equals("SqlConnection") || typeName.Equals("SqlClientFactory"))
                return Singleton<SqlServerDatabaseProvider>.Instance;
            if (typeName.StartsWith("FbConnection") || typeName.EndsWith("FirebirdClientFactory"))
                return Singleton<FirebirdDbDatabaseProvider>.Instance;
            if (typeName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0
                && (connectionString.IndexOf("Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                    connectionString.IndexOf("ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0))
            {
                return Singleton<MsAccessDbDatabaseProvider>.Instance;
            }

            if (!allowDefault)
                throw new ArgumentException("Could not match `" + type.FullName + "` to a provider.", "type");

            // Assume SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance;
        }

        /// <summary>
        ///     Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
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

            if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MySqlDatabaseProvider>.Instance;
            if (providerName.IndexOf("MariaDb", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<MariaDbDatabaseProvider>.Instance;
            if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerCEDatabaseProviders>.Instance;
            if (providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0
                || providerName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<PostgreSQLDatabaseProvider>.Instance;
            if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<OracleDatabaseProvider>.Instance;
            if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SQLiteDatabaseProvider>.Instance;
            if (providerName.IndexOf("Firebird", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("FbConnection", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<FirebirdDbDatabaseProvider>.Instance;
            if (providerName.IndexOf("OleDb", StringComparison.InvariantCultureIgnoreCase) >= 0
                && (connectionString.IndexOf("Jet.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                    connectionString.IndexOf("ACE.OLEDB", StringComparison.InvariantCultureIgnoreCase) > 0))
            {
                return Singleton<MsAccessDbDatabaseProvider>.Instance;
            }

            if (providerName.IndexOf("SqlServer", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                providerName.IndexOf("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return Singleton<SqlServerDatabaseProvider>.Instance;

            if (!allowDefault)
                throw new ArgumentException("Could not match `" + providerName + "` to a provider.", "providerName");

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
            var sp = factory as IServiceProvider;

            if (sp == null)
                return factory;

            try
            {
                var unwrapped = sp.GetService(factory.GetType()) as DbProviderFactory;
                return unwrapped == null ? factory : Unwrap(unwrapped);
            }
            catch (Exception)
            {
                return factory;
            }
        }

        protected void ExecuteNonQueryHelper(Database db, IDbCommand cmd)
        {
            db.DoPreExecute(cmd);
            cmd.ExecuteNonQuery();
            db.OnExecutedCommand(cmd);
        }

        protected object ExecuteScalarHelper(Database db, IDbCommand cmd)
        {
            db.DoPreExecute(cmd);
            object r = cmd.ExecuteScalar();
            db.OnExecutedCommand(cmd);
            return r;
        }
    }
}