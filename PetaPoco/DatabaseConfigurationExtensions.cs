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
            ((IBuildConfigurationSettings)source).SetSetting(key, value);
        }

        #region Timeout Settings

        /// <summary>
        /// Sets <see cref="IDatabase.CommandTimeout"/> to the specified value.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="seconds">The timeout in seconds.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException"><paramref name="seconds"/> is less than 1.</exception>
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
        /// Enables auto-select, equivalent to setting <see cref="IDatabase.EnableAutoSelect" /> to <see langword="true"/>.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithAutoSelect(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableAutoSelect, true);
            return source;
        }

        /// <summary>
        /// Disables auto-select, equivalent to setting <see cref="IDatabase.EnableAutoSelect" /> to <see langword="false"/>.
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
        /// Enables named parameters, equivalent to setting <see cref="IDatabase.EnableNamedParams" /> to <see langword="true"/>.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration WithNamedParams(this IDatabaseBuildConfiguration source)
        {
            source.SetSetting(EnableNamedParams, true);
            return source;
        }

        /// <summary>
        /// Disables named parameters, equivalent to setting <see cref="IDatabase.EnableNamedParams" /> to <see langword="false"/>.
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
        /// Specifies a connection string name to be used to locate a connection string. The <see cref="IDatabase.ConnectionString"/> and <see cref="IDatabase.Provider"/> will be read from the app or web configuration file.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionStringName">The name of the connection string to locate.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException"><paramref name="connectionStringName" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingConnectionStringName(this IDatabaseBuildConfiguration source, string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentException("Argument is null or empty", nameof(connectionStringName));
            source.SetSetting(ConnectionStringName, connectionStringName);
            return source;
        }
#endif

        /// <summary>
        /// Specifies a connection string to use.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="source">The configuration source.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException"><paramref name="connectionString" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingConnectionString(this IDatabaseBuildConfiguration source, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Argument is null or empty", nameof(connectionString));
            source.SetSetting(ConnectionString, connectionString);
            return source;
        }

        /// <summary>
        /// Specifies an existing <see cref="IDbConnection"/> to use.
        /// </summary>
        /// <remarks>
        /// The supplied IDbConnection will not be closed and disposed of by PetaPoco - that remains the responsibility of the caller.
        /// </remarks>
        /// <param name="source">The configuration source.</param>
        /// <param name="connection">The database connection.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="connection" /> is null.</exception>
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
        /// Specifies the provider name to be used when resolving the <see cref="IDatabase.Provider"/>.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <param name="providerName">The provider name to resolve.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentException"><paramref name="providerName" /> is null or empty.</exception>
        public static IDatabaseBuildConfiguration UsingProviderName(this IDatabaseBuildConfiguration source, string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentException("Argument is null or empty", nameof(providerName));
            source.SetSetting(ProviderName, providerName);
            return source;
        }

        /// <summary>
        /// Specifies the <see cref="IProvider"/> to use.
        /// </summary>
        /// <remarks>
        /// This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)" />.
        /// </remarks>
        /// <typeparam name="TProvider">The provider type, which must implement the the <see cref="IProvider"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingProvider<TProvider>(this IDatabaseBuildConfiguration source)
            where TProvider : class, IProvider, new()
        {
            source.SetSetting(Provider, new TProvider());
            return source;
        }

        /// <summary>
        /// Specifies the <see cref="IProvider"/> to use, with an accompanying configuration provider action.
        /// </summary>
        /// <remarks>
        /// This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)" />.
        /// </remarks>
        /// <typeparam name="TProvider">The provider type, which must implement the the <see cref="IProvider"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configurer">The action used to configure the provider.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configurer"/> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<TProvider>(this IDatabaseBuildConfiguration source, Action<TProvider> configurer)
            where TProvider : class, IProvider, new()
        {
            if (configurer == null)
                throw new ArgumentNullException(nameof(configurer));
            var provider = new TProvider();
            configurer(provider);
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        /// Specifies the <see cref="IProvider"/> to use.
        /// </summary>
        /// <remarks>
        /// This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)"/>.
        /// </remarks>
        /// <typeparam name="TProvider">The provider type, which must implement the the <see cref="IProvider"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="provider">The database provider.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<TProvider>(this IDatabaseBuildConfiguration source, TProvider provider)
            where TProvider : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            source.SetSetting(Provider, provider);
            return source;
        }

        /// <summary>
        /// Specifies the <see cref="IProvider"/> to use, with an accompanying configuration provider action.
        /// </summary>
        /// <remarks>
        /// This takes precedence over <see cref="UsingProviderName(IDatabaseBuildConfiguration, string)"/>.
        /// </remarks>
        /// <typeparam name="TProvider">The provider type, which must implement the the <see cref="IProvider"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configurer">The action used to configure the provider.</param>
        /// <param name="provider">The database provider.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> or <paramref name="configurer"/> is null.</exception>
        public static IDatabaseBuildConfiguration UsingProvider<TProvider>(this IDatabaseBuildConfiguration source, TProvider provider, Action<TProvider> configurer)
            where TProvider : class, IProvider
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (configurer == null)
                throw new ArgumentNullException(nameof(configurer));
            configurer(provider);
            source.SetSetting(Provider, provider);
            return source;
        }

        #endregion

        #region Mapper Settings

        /// <summary>
        /// Specifies the default <see cref="IMapper"/> to use when no specific mapper has been registered.
        /// </summary>
        /// <typeparam name="TMapper">The mapper type, which must implement the the <see cref="IMapper"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<TMapper>(this IDatabaseBuildConfiguration source)
            where TMapper : class, IMapper, new()
        {
            source.SetSetting(DefaultMapper, new TMapper());
            return source;
        }

        /// <summary>
        /// Specifies the default <see cref="IMapper"/> to use when no specific mapper has been registered, with an accompanying configuration mapper action.
        /// </summary>
        /// <typeparam name="TMapper">The mapper type, which must implement the the <see cref="IMapper"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="configurer">The action used to configure the mapper.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configurer"/> action is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<TMapper>(this IDatabaseBuildConfiguration source, Action<TMapper> configurer)
            where TMapper : class, IMapper, new()
        {
            if (configurer == null)
                throw new ArgumentNullException(nameof(configurer));
            var mapper = new TMapper();
            configurer(mapper);
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        /// Specifies the default <see cref="IMapper"/> to use when no specific mapper has been registered.
        /// </summary>
        /// <typeparam name="TMapper">The mapper type, which must implement the the <see cref="IMapper"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<TMapper>(this IDatabaseBuildConfiguration source, TMapper mapper)
            where TMapper : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));
            source.SetSetting(DefaultMapper, mapper);
            return source;
        }

        /// <summary>
        /// Specifies the default <see cref="IMapper"/> to use when no specific mapper has been registered, with an accompanying configuration mapper action.
        /// </summary>
        /// <typeparam name="TMapper">The mapper type, which must implement the the <see cref="IMapper"/> interface.</typeparam>
        /// <param name="source">The configuration source.</param>
        /// <param name="mapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <param name="configurer">The action used to configure the mapper.</param>
        /// <returns>The original <paramref name="source"/> configuration, to form a fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mapper"/> or <paramref name="configurer"/> is null.</exception>
        public static IDatabaseBuildConfiguration UsingDefaultMapper<TMapper>(this IDatabaseBuildConfiguration source, TMapper mapper, Action<TMapper> configurer)
            where TMapper : class, IMapper
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));
            if (configurer == null)
                throw new ArgumentNullException(nameof(configurer));
            configurer(mapper);
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
        /// Specifies an event handler to use when a transaction has been started.
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
        /// Specifies an event handler to use when a connection is about to be opened.
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
