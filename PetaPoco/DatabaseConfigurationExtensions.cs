using System;
using System.Data;
using PetaPoco.Core;

namespace PetaPoco
{
    /// <summary>
    /// Extension methods for <see cref="IDatabaseBuildConfiguration" />, and the fluent interface integration.
    /// </summary>
    public static class DatabaseConfigurationExtensions
    {
        internal const string CommandTimeout = "CommandTimeout";
        internal const string EnableAutoSelect = "EnableAutoSelect";
        internal const string EnableNamedParams = "EnableNamedParams";

#if !NETSTANDARD
        internal const string ConnectionStringName = "ConnectionStringName";
#endif
        internal const string ConnectionString = "ConnectionString";
        internal const string ProviderName = "ProviderName";
        internal const string Provider = "Provider";

        internal const string DefaultMapper = "DefaultMapper";
        internal const string IsolationLevel = "IsolationLevel";
        internal const string TransactionStarted = "TransactionStarted";
        internal const string TransactionEnding = "TransactionEnding";
        internal const string CommandExecuting = "CommandExecuting";
        internal const string CommandExecuted = "CommandExecuted";
        internal const string ConnectionOpening = "ConnectionOpening";
        internal const string ConnectionOpened = "ConnectionOpened";
        internal const string ConnectionClosing = "ConnectionClosing";
        internal const string ExceptionThrown = "ExceptionThrown";
        internal const string Connection = "Connection";

        private static void SetSetting(this IDatabaseBuildConfiguration source, string key, object value)
        {
            ((IBuildConfigurationSettings) source).SetSetting(key, value);
        }

        #region Timeout Settings

        /// <summary>
        /// Adds a command timeout - see <see cref="IDatabase.CommandTimeout" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="seconds">The timeout in seconds.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException">Thrown when seconds is less than 1.</exception>
        public static IDatabaseBuildConfiguration UsingCommandTimeout(this IDatabaseBuildConfiguration source, int seconds)
        {
            if (seconds < 1)
                throw new ArgumentException("Timeout value must be greater than zero.");
            source.SetSetting(CommandTimeout, seconds);
            return source;
        }

        #endregion

        #region AutoSelect Settings

        /// <summary>
        /// Enables auto select <see cref="IDatabase.EnableAutoSelect" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithAutoSelect(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableAutoSelect, true);
            return source;
        }

        /// <summary>
        /// Disables auto select - see <see cref="IDatabase.EnableAutoSelect" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithoutAutoSelect(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableAutoSelect, false);
            return source;
        }

        #endregion

        #region NamedParams Settings

        /// <summary>
        /// Enables named params - see <see cref="IDatabase.EnableNamedParams" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithNamedParams(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableNamedParams, true);
            return source;
        }

        /// <summary>
        /// Disables named params - see <see cref="IDatabase.EnableNamedParams" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithoutNamedParams(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableNamedParams, false);
            return source;
        }

        #endregion

        #region Connection Settings

#if !NETSTANDARD
        /// <summary>
        /// Adds a connection string name.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionStringName">The connection string name.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionStringName" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingConnectionStringName(this IDatabaseBuildConfiguration source, string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentException("Argument is null or empty", nameof(connectionStringName));
            source.SetSetting(ConnectionStringName, connectionStringName);
            return source;
        }
#endif

        /// <summary>
        /// Adds a connection string - see <see cref="IDatabase.ConnectionString" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingConnectionString(this IDatabaseBuildConfiguration source, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Argument is null or empty", nameof(connectionString));
            source.SetSetting(ConnectionString, connectionString);
            return source;
        }

        /// <summary>
        /// Specifies an <see cref="IDbConnection" /> to use.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="connection">The connection to use.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnection(this IDatabaseBuildConfiguration source, IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            source.SetSetting(Connection, connection);
            return source;
        }

        #endregion

        #region Provider Settings

        /// <summary>
        /// Adds a provider name string - see <see cref="DatabaseProvider.Resolve(string, bool, string)" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="providerName">The provider name.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="providerName" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingProviderName(this IDatabaseBuildConfiguration source, string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentException("Argument is null or empty", nameof(providerName));
            source.SetSetting(ProviderName, providerName);
            return source;
        }

        /// <summary>
        /// Specifies the provider to be used. - see <see cref="IDatabase.Provider" />. This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)" />.
        /// </summary>
        /// <typeparam name="T">The provider type.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source) where T : class, IProvider, new()
        {
            source.SetSetting(Provider, new T());
            return source;
        }

        /// <summary>
        /// Specifies the <see cref="IDatabase.Provider" /> to use, with an accompanying configuration provider custom callback.
        /// </summary>
        /// <typeparam name="T">The provider type, constrained to a class deriving from <see cref="IProvider"/>.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">The configure provider callback.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, Action<T> configure) where T : class, IProvider, new()
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            var provider = new T();
            configure(provider);
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        /// Specifies the provider to be used. - see <see cref="IDatabase.Provider" />. This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)" />.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, T provider) where T : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        /// Specifies the provider to be used. - see <see cref="IDatabase.Provider" />. This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)" />.
        /// </summary>
        /// <typeparam name="T">The provider type.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">The configure provider callback.</param>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<T>(this IDatabaseBuildConfiguration source, T provider, Action<T> configure) where T : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            configure(provider);
            source.SetSetting(Provider, provider);
            return source;
        }

        #endregion

        #region Mapper Settings

        /// <summary>
        /// Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <typeparam name="T">The mapper type.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source) where T : class, IMapper, new()
        {
            source.SetSetting(DefaultMapper, new T());
            return source;
        }

        /// <summary>
        /// Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <typeparam name="T">The type of the mapper. This type must be a class that implements the <see cref="IMapper"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configure">A callback function to configure the mapper.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="configure"/> callback function is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, Action<T> configure) where T : class, IMapper, new()
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            var mapper = new T();
            configure(mapper);
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        /// Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The mapper to use as the default.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapper" /> is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, T mapper) where T : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        /// Specifies the default mapper to use when no specific mapper has been registered.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The mapper to use as the default.</param>
        /// <param name="configure">The configure mapper callback.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapper" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure" /> is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<T>(this IDatabaseBuildConfiguration source, T mapper, Action<T> configure) where T : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            configure(mapper);
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        #endregion

        #region Transaction Settings

        /// <summary>
        /// Specifies the transaction isolation level to use.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingIsolationLevel(this IDatabaseBuildConfiguration source, IsolationLevel isolationLevel)
        {
            source.SetSetting(IsolationLevel, isolationLevel);
            return source;
        }

        #endregion

        #region Event Settings

        /// <summary>
        /// Specifies an event handler to use when a new transaction has been started.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.TransactionStarted"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingTransactionStarted(this IDatabaseBuildConfiguration source, EventHandler<DbTransactionEventArgs> handler)
        {
            source.SetSetting(TransactionStarted, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a transaction is about to be rolled back or committed.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.TransactionEnding"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingTransactionEnding(this IDatabaseBuildConfiguration source, EventHandler<DbTransactionEventArgs> handler)
        {
            source.SetSetting(TransactionEnding, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a database command is about to be executed.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.CommandExecuting"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingCommandExecuting(this IDatabaseBuildConfiguration source, EventHandler<DbCommandEventArgs> handler)
        {
            source.SetSetting(CommandExecuting, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a database command has been executed.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.CommandExecuted"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingCommandExecuted(this IDatabaseBuildConfiguration source, EventHandler<DbCommandEventArgs> handler)
        {
            source.SetSetting(CommandExecuted, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use before a connection is opened.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.ConnectionOpening"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnectionOpening(this IDatabaseBuildConfiguration source, EventHandler<DbConnectionEventArgs> handler)
        {
            source.SetSetting(ConnectionOpening, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a database connection has been opened.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.ConnectionOpened"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnectionOpened(this IDatabaseBuildConfiguration source, EventHandler<DbConnectionEventArgs> handler)
        {
            source.SetSetting(ConnectionOpened, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a database connection is about to be closed.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.ConnectionClosing"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingConnectionClosing(this IDatabaseBuildConfiguration source, EventHandler<DbConnectionEventArgs> handler)
        {
            source.SetSetting(ConnectionClosing, handler);
            return source;
        }

        /// <summary>
        /// Specifies an event handler to use when a database exception has been thrown.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="handler">A callback function for handling <see cref="IDatabase.ExceptionThrown"/> events.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingExceptionThrown(this IDatabaseBuildConfiguration source, EventHandler<ExceptionEventArgs> handler)
        {
            source.SetSetting(ExceptionThrown, handler);
            return source;
        }

        #endregion

        #region Finalize Fluent Configuration

        /// <summary>
        /// Creates an instance of PetaPoco using the specified <paramref name="source" />.
        /// </summary>
        /// <param name="source">The configuration source used to create and configure an instance of PetaPoco.</param>
        /// <returns>An instance of PetaPoco.</returns>
        public static IDatabase Create(this IDatabaseBuildConfiguration source)
        {
            return new Database(source);
        }

        #endregion
    }
}
