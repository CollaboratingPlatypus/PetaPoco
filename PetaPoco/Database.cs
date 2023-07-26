using System;
using System.Collections.Generic;
#if !NETSTANDARD
using System.Configuration;
#endif
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;

namespace PetaPoco
{
    /// <inheritdoc cref="IDatabase"/>
    public partial class Database : IDatabase
    {
        #region Member Fields

        private IMapper _defaultMapper;
        private string _connectionString;
        private IProvider _provider;
        private IsolationLevel? _isolationLevel;
        private IDbConnection _sharedConnection;
        private IDbTransaction _transaction;
        private DbProviderFactory _factory;
        private int _sharedConnectionDepth;
        private int _transactionDepth;
        private bool _transactionCancelled;
        private string _paramPrefix;
        private string _lastSql;
        private object[] _lastArgs;

        #endregion

        #region Constructors

#if !NETSTANDARD
        /// <summary>
        /// Constructs an instance using the first connection string found in the app/web configuration file.
        /// </summary>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="InvalidOperationException">Thrown when no connection strings can registered.</exception>
        public Database(IMapper defaultMapper = null)
        {
            if (ConfigurationManager.ConnectionStrings.Count == 0)
                throw new InvalidOperationException("One or more connection strings must be registered to use the no-parameter constructor");

            var entry = ConfigurationManager.ConnectionStrings[0];
            _connectionString = entry.ConnectionString;
            InitialiseFromEntry(entry, defaultMapper);
        }

        /// <summary>
        /// Constructs an instance using a supplied connection string name. The actual connection string and provider will be read from app/web.config.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionStringName" /> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection string cannot be found.</exception>
        public Database(string connectionStringName, IMapper defaultMapper = null)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentException("Connection string name must not be null or empty", nameof(connectionStringName));

            var entry = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (entry == null)
                throw new InvalidOperationException(string.Format("Can't find a connection string with the name '{0}'", connectionStringName));

            _connectionString = entry.ConnectionString;
            InitialiseFromEntry(entry, defaultMapper);
        }

        private void InitialiseFromEntry(ConnectionStringSettings entry, IMapper defaultMapper)
        {
            var providerName = !string.IsNullOrEmpty(entry.ProviderName) ? entry.ProviderName : "System.Data.SqlClient";
            Initialise(DatabaseProvider.Resolve(providerName, false, _connectionString), defaultMapper);
        }
#endif

        /// <summary>
        /// Constructs an instance using a supplied IDbConnection.
        /// </summary>
        /// <remarks>
        /// The supplied IDbConnection will not be closed and disposed of by PetaPoco - that remains the responsibility of the caller.
        /// </remarks>
        /// <param name="connection">The IDbConnection to use.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connection" /> is null or empty.</exception>
        public Database(IDbConnection connection, IMapper defaultMapper = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            SetupFromConnection(connection);
            Initialise(DatabaseProvider.Resolve(_sharedConnection.GetType(), false, _connectionString), defaultMapper);
        }

        /// <summary>
        /// Constructs an instance using a supplied connection string and provider name.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="providerName">The database provider name.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        public Database(string connectionString, string providerName, IMapper defaultMapper = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string must not be null or empty", nameof(connectionString));
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentException("Provider name must not be null or empty", nameof(providerName));

            _connectionString = connectionString;
            Initialise(DatabaseProvider.Resolve(providerName, false, _connectionString), defaultMapper);
        }

        /// <summary>
        /// Constructs an instance using the supplied connection string and DbProviderFactory.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="factory">The DbProviderFactory to use for instantiating IDbConnections.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory" /> is null.</exception>
        public Database(string connectionString, DbProviderFactory factory, IMapper defaultMapper = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string must not be null or empty", nameof(connectionString));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _connectionString = connectionString;
            Initialise(DatabaseProvider.Resolve(DatabaseProvider.Unwrap(factory).GetType(), false, _connectionString), defaultMapper);
        }

        /// <summary>
        /// Constructs an instance using the supplied provider and optional default mapper.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="provider">The provider to use.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider" /> is null.</exception>
        public Database(string connectionString, IProvider provider, IMapper defaultMapper = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string must not be null or empty", nameof(connectionString));

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _connectionString = connectionString;
            Initialise(provider, defaultMapper);
        }

        /// <summary>
        /// Constructs an instance using the supplied <paramref name="configuration" />.
        /// </summary>
        /// <param name="configuration">The configuration for constructing an instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration" /> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no configuration string is configured and there are no connection strings registered in the app/web config.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection string configured and no provider is configured.</exception>
        public Database(IDatabaseBuildConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var settings = (IBuildConfigurationSettings)configuration;

            IMapper defaultMapper = null;
            settings.TryGetSetting<IMapper>(DatabaseConfigurationExtensions.DefaultMapper, v => defaultMapper = v);

            IProvider provider = null;
            IDbConnection connection = null;
            string providerName = null;
#if !NETSTANDARD
            ConnectionStringSettings entry = null;
#endif

            settings.TryGetSetting<IProvider>(DatabaseConfigurationExtensions.Provider, p => provider = p);
            settings.TryGetSetting<IDbConnection>(DatabaseConfigurationExtensions.Connection, c => connection = c);
            settings.TryGetSetting<string>(DatabaseConfigurationExtensions.ProviderName, pn => providerName = pn);

            if (connection != null)
            {
                SetupFromConnection(connection);
            }
            else
            {
                settings.TryGetSetting<string>(DatabaseConfigurationExtensions.ConnectionString, cs => _connectionString = cs);

#if !NETSTANDARD
                if (_connectionString == null)
                {
                    string connectionStringName = null;
                    settings.TryGetSetting<string>(DatabaseConfigurationExtensions.ConnectionStringName, n => connectionStringName = n);

                    if (connectionStringName != null)
                    {
                        entry = ConfigurationManager.ConnectionStrings[connectionStringName];
                        if (entry == null)
                            throw new InvalidOperationException($"Can't find a connection string with the name '{connectionStringName}'");
                    }
                    else
                    {
                        if (ConfigurationManager.ConnectionStrings.Count == 0)
                            throw new InvalidOperationException("One or more connection strings must be registered, when not providing a connection string");

                        entry = ConfigurationManager.ConnectionStrings[0];
                    }

                    _connectionString = entry.ConnectionString;
                }
#else
                if (_connectionString == null)
                    throw new InvalidOperationException("A connection string is required.");
#endif
            }

            if (provider != null)
                Initialise(provider, defaultMapper);
            else if (providerName != null)
                Initialise(DatabaseProvider.Resolve(providerName, false, _connectionString), defaultMapper);
#if !NETSTANDARD
            else if (entry != null)
                InitialiseFromEntry(entry, defaultMapper);
#endif
            else if (connection != null)
                Initialise(DatabaseProvider.Resolve(_sharedConnection.GetType(), false, _connectionString), defaultMapper);
            else
                throw new InvalidOperationException("Unable to locate a provider.");

            settings.TryGetSetting<bool>(DatabaseConfigurationExtensions.EnableNamedParams, v => EnableNamedParams = v);
            settings.TryGetSetting<bool>(DatabaseConfigurationExtensions.EnableAutoSelect, v => EnableAutoSelect = v);
            settings.TryGetSetting<int>(DatabaseConfigurationExtensions.CommandTimeout, v => CommandTimeout = v);
            settings.TryGetSetting<IsolationLevel>(DatabaseConfigurationExtensions.IsolationLevel, v => IsolationLevel = v);

            settings.TryGetSetting<EventHandler<DbConnectionEventArgs>>(DatabaseConfigurationExtensions.ConnectionOpening, v => ConnectionOpening += v);
            settings.TryGetSetting<EventHandler<DbConnectionEventArgs>>(DatabaseConfigurationExtensions.ConnectionOpened, v => ConnectionOpened += v);
            settings.TryGetSetting<EventHandler<DbConnectionEventArgs>>(DatabaseConfigurationExtensions.ConnectionClosing, v => ConnectionClosing += v);
            settings.TryGetSetting<EventHandler<DbTransactionEventArgs>>(DatabaseConfigurationExtensions.TransactionStarted, v => TransactionStarted += v);
            settings.TryGetSetting<EventHandler<DbTransactionEventArgs>>(DatabaseConfigurationExtensions.TransactionEnding, v => TransactionEnding += v);
            settings.TryGetSetting<EventHandler<DbCommandEventArgs>>(DatabaseConfigurationExtensions.CommandExecuting, v => CommandExecuting += v);
            settings.TryGetSetting<EventHandler<DbCommandEventArgs>>(DatabaseConfigurationExtensions.CommandExecuted, v => CommandExecuted += v);
            settings.TryGetSetting<EventHandler<ExceptionEventArgs>>(DatabaseConfigurationExtensions.ExceptionThrown, v => ExceptionThrown += v);
        }

        private void SetupFromConnection(IDbConnection connection)
        {
            _sharedConnection = connection;
            _connectionString = connection.ConnectionString;

            // Prevent closing external connection
            _sharedConnectionDepth = 2;
        }

        /// <summary>
        /// Provides common initialization for the various constructors.
        /// </summary>
        private void Initialise(IProvider provider, IMapper mapper)
        {
            // Reset
            _transactionDepth = 0;
            EnableAutoSelect = true;
            EnableNamedParams = true;

            // What character is used for delimiting parameters in SQL
            _provider = provider;
            _paramPrefix = _provider.GetParameterPrefix(_connectionString);
            _factory = _provider.GetFactory();

            _defaultMapper = mapper ?? new ConventionMapper();
        }

        #endregion

        #region Connection Management (IConnection Implementation)

        /// <summary>
        /// Provides access to the currently open shared connection.
        /// </summary>
        /// <returns>The currently open connection, or <see langword="null"/>.</returns>
        /// <seealso cref="OpenSharedConnection" />
        /// <seealso cref="CloseSharedConnection" />
        /// <seealso cref="KeepConnectionAlive" />
        public IDbConnection Connection => _sharedConnection;

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="OpenSharedConnection" /> and <see cref="CloseSharedConnection" /> are reference counted and should be balanced
        /// </remarks>
        /// <seealso cref="Connection" />
        /// <seealso cref="CloseSharedConnection" />
        /// <seealso cref="KeepConnectionAlive" />
        public void OpenSharedConnection()
        {
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _connectionString;

                _sharedConnection = OnConnectionOpening(_sharedConnection);

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open();

                _sharedConnection = OnConnectionOpened(_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++;
            }

            _sharedConnectionDepth++;
        }

#if ASYNC
        /// <summary>
        /// The async version of <see cref="OpenSharedConnection" />.
        /// </summary>
        public Task OpenSharedConnectionAsync()
            => OpenSharedConnectionAsync(CancellationToken.None);

        /// <summary>
        /// The async version of <see cref="OpenSharedConnection" />.
        /// </summary>
        public async Task OpenSharedConnectionAsync(CancellationToken cancellationToken)
        {
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _connectionString;

                _sharedConnection = OnConnectionOpening(_sharedConnection);

                if (_sharedConnection.State == ConnectionState.Broken)
                    _sharedConnection.Close();

                if (_sharedConnection.State == ConnectionState.Closed)
                {
                    var con = _sharedConnection as DbConnection;
                    if (con != null)
                        await con.OpenAsync(cancellationToken).ConfigureAwait(false);
                    else
                        _sharedConnection.Open();
                }

                _sharedConnection = OnConnectionOpened(_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++;
            }

            _sharedConnectionDepth++;
        }
#endif

        /// <summary>
        /// Releases the shared connection.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="OpenSharedConnection" /> and <see cref="CloseSharedConnection" /> are reference counted and should be balanced
        /// </remarks>
        /// <seealso cref="Connection" />
        /// <seealso cref="KeepConnectionAlive" />
        /// <seealso cref="OpenSharedConnection" />
        public void CloseSharedConnection()
        {
            if (_sharedConnectionDepth > 0)
            {
                _sharedConnectionDepth--;
                if (_sharedConnectionDepth == 0)
                {
                    OnConnectionClosing(_sharedConnection);
                    _sharedConnection.Dispose();
                    _sharedConnection = null;
                }
            }
        }

        /// <summary>
        /// Alias for <see cref="CloseSharedConnection" />.
        /// </summary>
        /// <remarks>
        /// Called implicitly when making use of the .NET `using` language feature.
        /// </remarks>
        public void Dispose()
        {
            // Automatically close one open connection reference
            //  (Works with KeepConnectionAlive and manually opening a shared connection)
            CloseSharedConnection();
        }

        #endregion

        #region Transaction Management (ITransactionAccessor, IDatabase implementation)

        /// <inheritdoc/>
        IDbTransaction ITransactionAccessor.Transaction => _transaction;

        /// <inheritdoc/>
        public ITransaction GetTransaction()
            => new Transaction(this);

        /// <summary>
        /// Called when a transaction starts.
        /// </summary>
        /// <seealso cref="BeginTransaction"/>
        public virtual void OnBeginTransaction()
        {
            TransactionStarted?.Invoke(this, new DbTransactionEventArgs(_transaction));
        }

        /// <summary>
        /// Called when a transaction ends.
        /// </summary>
        public virtual void OnEndTransaction()
        {
            TransactionEnding?.Invoke(this, new DbTransactionEventArgs(_transaction));
        }

        /// <inheritdoc/>
        public void BeginTransaction()
        {
            _transactionDepth++;

            if (_transactionDepth == 1)
            {
                OpenSharedConnection();
                _transaction = !_isolationLevel.HasValue ? _sharedConnection.BeginTransaction() : _sharedConnection.BeginTransaction(_isolationLevel.Value);
                _transactionCancelled = false;
                OnBeginTransaction();
            }
        }

#if ASYNC
        /// <inheritdoc/>
        public Task BeginTransactionAsync()
            => BeginTransactionAsync(CancellationToken.None);

        /// <inheritdoc/>
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_sharedConnection is DbConnection asyncConn)
            {
                _transactionDepth++;

                if (_transactionDepth == 1)
                {
                    await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                    _transaction = !_isolationLevel.HasValue
#if NETSTANDARD2_1
                        ? await asyncConn.BeginTransactionAsync().ConfigureAwait(false)
                        : await asyncConn.BeginTransactionAsync(_isolationLevel.Value).ConfigureAwait(false);
#else
                        ? _sharedConnection.BeginTransaction()
                        : _sharedConnection.BeginTransaction(_isolationLevel.Value);
#endif
                    _transactionCancelled = false;
                    OnBeginTransaction();
                }
            }
            else
            {
                BeginTransaction();
            }
        }
#endif

        /// <inheritdoc/>
        public void AbortTransaction()
        {
            _transactionCancelled = true;
            CompleteTransaction();
        }

#if ASYNC
        public Task AbortTransactionAsync()
        {
            this._transactionCancelled = true;
            return CompleteTransactionAsync();
        }
#endif

        /// <inheritdoc/>
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

#if ASYNC
        public async Task CompleteTransactionAsync()
        {
            if ((--_transactionDepth) == 0)
                await CleanupTransactionAsync().ConfigureAwait(false);
        }
#endif

        /// <summary>
        /// Internal helper to cleanup transaction.
        /// </summary>
        private void CleanupTransaction()
        {
            OnEndTransaction();

            if (_transactionCancelled)
                _transaction.Rollback();
            else
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;

            CloseSharedConnection();
        }

#if ASYNC
#pragma warning disable 1998
        private async Task CleanupTransactionAsync()
        {
#if NETSTANDARD2_1
            if (_transaction is DbTransaction asyncTrans)
            {
                OnEndTransaction();

                if (_transactionCancelled)
                    await asyncTrans.RollbackAsync().ConfigureAwait(false);
                else
                    await asyncTrans.CommitAsync().ConfigureAwait(false);

                _transaction.Dispose();
                _transaction = null;

                CloseSharedConnection();
            }
            else
#endif
            {
                CleanupTransaction();
            }
        }
#pragma warning restore 1998
#endif

        #endregion

        #region Exception Reporting and Logging

        /// <summary>
        /// Called if an exception occurs during processing of a DB operation.  Override to provide custom logging/handling.
        /// </summary>
        /// <param name="ex">The exception instance.</param>
        /// <returns><see langword="true"/> to re-throw the exception, <see langword="false"/> to suppress it.</returns>
        public virtual bool OnException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            System.Diagnostics.Debug.WriteLine(LastCommand);

            var args = new ExceptionEventArgs(ex);
            ExceptionThrown?.Invoke(this, new ExceptionEventArgs(ex));
            return args.Raise;
        }

        /// <summary>
        /// Called when DB connection opened.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of opened connections, or to provide a proxy IDbConnection.
        /// </remarks>
        /// <param name="conn">The newly-opened IDbConnection.</param>
        /// <returns>The same or a replacement IDbConnection.</returns>
        public virtual IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            var args = new DbConnectionEventArgs(conn);
            ConnectionOpened?.Invoke(this, args);
            return args.Connection;
        }

        /// <summary>
        /// Called before a DB connection is opened.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of opening connections, or to provide a proxy IDbConnection.
        /// </remarks>
        /// <param name="conn">The soon-to-be-opened IDbConnection.</param>
        /// <returns>The same or a replacement IDbConnection.</returns>
        public virtual IDbConnection OnConnectionOpening(IDbConnection conn)
        {
            var args = new DbConnectionEventArgs(conn);
            ConnectionOpening?.Invoke(this, args);
            return args.Connection;
        }

        /// <summary>
        /// Called when DB connection closed.
        /// </summary>
        /// <param name="conn">The soon-to-be-closed IDBConnection.</param>
        public virtual void OnConnectionClosing(IDbConnection conn)
        {
            ConnectionClosing?.Invoke(this, new DbConnectionEventArgs(conn));
        }

        /// <summary>
        /// Called just before an DB command is executed.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of commands, modification of the IDbCommand before it's executed, or any other custom actions that should be performed before every command
        /// </remarks>
        /// <param name="cmd">The command to be executed.</param>
        public virtual void OnExecutingCommand(IDbCommand cmd)
        {
            CommandExecuting?.Invoke(this, new DbCommandEventArgs(cmd));
        }

        /// <summary>
        /// Called on completion of command execution.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging or other actions after every command has completed.
        /// </remarks>
        /// <param name="cmd">The IDbCommand that finished executing.</param>
        public virtual void OnExecutedCommand(IDbCommand cmd)
        {
            CommandExecuted?.Invoke(this, new DbCommandEventArgs(cmd));
        }

        #endregion

        #region ExecuteNonQuery, ExecuteAsync

        /// <inheritdoc/>
        public int Execute(Sql sql)
            => Execute(sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public int Execute(string sql, params object[] args)
            => ExecuteInternal(CommandType.Text, sql, args);

#if ASYNC
        public Task<int> ExecuteAsync(Sql sql)
            => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        public Task<int> ExecuteAsync(string sql, params object[] args)
            => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql, args);

        public Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql)
            => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        public Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args)
            => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql, args);
#endif

        protected virtual int ExecuteInternal(CommandType commandType, string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        return ExecuteNonQueryHelper(cmd);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return -1;
            }
        }

#if ASYNC
        protected virtual async Task<int> ExecuteInternalAsync(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        return await ExecuteNonQueryHelperAsync(cancellationToken, cmd).ConfigureAwait(false);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return -1;
            }
        }
#endif

        #endregion

        #region ExecuteScalar, ExecuteScalarAsync

        /// <inheritdoc/>
        public T ExecuteScalar<T>(Sql sql)
            => ExecuteScalar<T>(sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public T ExecuteScalar<T>(string sql, params object[] args)
            => ExecuteScalarInternal<T>(CommandType.Text, sql, args);

#if ASYNC
        /// <inheritdoc/>
        public Task<T> ExecuteScalarAsync<T>(Sql sql)
            => ExecuteScalarInternalAsync<T>(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
            => ExecuteScalarInternalAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, Sql sql)
            => ExecuteScalarInternalAsync<T>(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => ExecuteScalarInternalAsync<T>(cancellationToken, CommandType.Text, sql, args);
#endif

        protected virtual T ExecuteScalarInternal<T>(CommandType commandType, string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var val = ExecuteScalarHelper(cmd);

                        // Handle nullable types
                        var u = Nullable.GetUnderlyingType(typeof(T));
                        if (u != null && (val == null || val == DBNull.Value))
                            return default(T);

                        return (T) Convert.ChangeType(val, u == null ? typeof(T) : u);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return default(T);
            }
        }

#if ASYNC
        protected virtual async Task<T> ExecuteScalarInternalAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var val = await ExecuteScalarHelperAsync(cancellationToken, cmd).ConfigureAwait(false);

                        var u = Nullable.GetUnderlyingType(typeof(T));
                        if (u != null && (val == null || val == DBNull.Value))
                            return default(T);

                        return (T) Convert.ChangeType(val, u == null ? typeof(T) : u);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return default(T);
            }
        }
#endif

        #endregion

        #region Query, QueryAsync : Single-Poco

        /// <inheritdoc/>
        public IEnumerable<T> Query<T>()
            => Query<T>(string.Empty);

        /// <inheritdoc/>
        public IEnumerable<T> Query<T>(Sql sql)
            => Query<T>(sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return ExecuteReader<T>(CommandType.Text, sql, args);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>()
            => QueryAsync<T>(CancellationToken.None, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(Sql sql)
            => QueryAsync<T>(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(string sql, params object[] args)
            => QueryAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken)
            => QueryAsync<T>(cancellationToken, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, Sql sql)
            => QueryAsync<T>(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => QueryAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType)
            => QueryAsync<T>(CancellationToken.None, commandType, string.Empty);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, Sql sql)
            => QueryAsync<T>(CancellationToken.None, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, string sql, params object[] args)
            => QueryAsync<T>(CancellationToken.None, commandType, sql, args);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType)
            => QueryAsync<T>(cancellationToken, commandType, string.Empty);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql)
            => QueryAsync<T>(cancellationToken, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return ExecuteReaderAsync<T>(cancellationToken, commandType, sql, args);
        }

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback)
            => QueryAsync(receivePocoCallback, CancellationToken.None, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, Sql sql)
            => QueryAsync(receivePocoCallback, CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken)
            => QueryAsync(receivePocoCallback, cancellationToken, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, Sql sql)
            => QueryAsync(receivePocoCallback, cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType)
            => QueryAsync(receivePocoCallback, CancellationToken.None, commandType, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, Sql sql)
            => QueryAsync(receivePocoCallback, CancellationToken.None, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, CancellationToken.None, commandType, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType)
            => QueryAsync(receivePocoCallback, cancellationToken, commandType, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType, Sql sql)
            => QueryAsync(receivePocoCallback, cancellationToken, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);
            return ExecuteReaderAsync(receivePocoCallback, cancellationToken, commandType, sql, args);
        }

#endif

        protected virtual IEnumerable<T> ExecuteReader<T>(CommandType commandType, string sql, params object[] args)
        {
            OpenSharedConnection();
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                {
                    IDataReader r;
                    var pd = PocoData.ForType(typeof(T), _defaultMapper);
                    try
                    {
                        r = ExecuteReaderHelper(cmd);
                    }
                    catch (Exception x)
                    {
                        if (OnException(x))
                            throw;
                        yield break;
                    }

                    var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, r.FieldCount, r,
                        _defaultMapper) as Func<IDataReader, T>;
                    using (r)
                    {
                        while (true)
                        {
                            T poco;
                            try
                            {
                                if (!r.Read())
                                    yield break;
                                poco = factory(r);
                            }
                            catch (Exception x)
                            {
                                if (OnException(x))
                                    throw;
                                yield break;
                            }

                            yield return poco;
                        }
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }

#if ASYNC
        protected virtual async Task<IAsyncReader<T>> ExecuteReaderAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, object[] args)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            var cmd = CreateCommand(_sharedConnection, commandType, sql, args);
            IDataReader reader = null;
            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            try
            {
                reader = await ExecuteReaderHelperAsync(cancellationToken, cmd).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (OnException(e))
                    throw;
                try
                {
                    cmd?.Dispose();
                    reader?.Dispose();
                }
                catch
                {
                    // ignored
                }

                return AsyncReader<T>.Empty();
            }

            var factory =
                pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, reader.FieldCount, reader, _defaultMapper) as Func<IDataReader, T>;

            return new AsyncReader<T>(this, cmd, reader, factory);
        }

        protected virtual async Task ExecuteReaderAsync<T>(Action<T> processPoco, CancellationToken cancellationToken, CommandType commandType, string sql, object[] args)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                {
                    IDataReader reader;
                    var pd = PocoData.ForType(typeof(T), _defaultMapper);

                    try
                    {
                        reader = await ExecuteReaderHelperAsync(cancellationToken, cmd).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (OnException(e))
                            throw;
                        return;
                    }

                    var readerAsync = reader as DbDataReader;
                    var factory =
                        pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, reader.FieldCount, reader,
                            _defaultMapper) as Func<IDataReader, T>;

                    using (reader)
                    {
                        while (true)
                        {
                            T poco;
                            try
                            {
                                if (readerAsync != null)
                                {
                                    if (!await readerAsync.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        return;
                                }
                                else
                                {
                                    if (!reader.Read())
                                        return;
                                }

                                poco = factory(reader);
                                processPoco(poco);
                            }
                            catch (Exception e)
                            {
                                if (OnException(e))
                                    throw;
                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }
#endif

        #endregion

        #region Query : Multi-Poco

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2) }, null, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql.SQL, sql.Arguments);

        public IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, null, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2>(string sql, params object[] args)
            => Query<T1>(new[] { typeof(T1), typeof(T2) }, null, sql, args);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql, args);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql, args);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5>(string sql, params object[] args)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, null, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
            => Query<TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, cb, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
            => Query<TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, args);

        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args)
            => Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, cb, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args)
        {
            OpenSharedConnection();
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, sql, args))
                {
                    IDataReader r;
                    try
                    {
                        r = ExecuteReaderHelper(cmd);
                    }
                    catch (Exception x)
                    {
                        if (OnException(x))
                            throw;
                        yield break;
                    }

                    var factory = MultiPocoFactory.GetFactory<TRet>(types, _sharedConnection.ConnectionString, sql, r, _defaultMapper);
                    if (cb == null)
                        cb = MultiPocoFactory.GetAutoMapper(types.ToArray());
                    var bNeedTerminator = false;
                    using (r)
                    {
                        while (true)
                        {
                            TRet poco;
                            try
                            {
                                if (!r.Read())
                                    break;
                                poco = factory(r, cb);
                            }
                            catch (Exception x)
                            {
                                if (OnException(x))
                                    throw;
                                yield break;
                            }

                            if (poco != null)
                                yield return poco;
                            else
                                bNeedTerminator = true;
                        }

                        if (bNeedTerminator)
                        {
                            var poco = (TRet) (cb as Delegate).DynamicInvoke(new object[types.Length]);
                            if (poco != null)
                                yield return poco;
                            else
                                yield break;
                        }
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }

        #endregion

        #region QueryMultiple : Multi-POCO Result Set IGridReader

        public IGridReader QueryMultiple(Sql sql)
            => QueryMultiple(sql.SQL, sql.Arguments);

        public IGridReader QueryMultiple(string sql, params object[] args)
        {
            OpenSharedConnection();

            GridReader result = null;

            var cmd = CreateCommand(_sharedConnection, sql, args);

            try
            {
                var reader = ExecuteReaderHelper(cmd);
                result = new GridReader(this, cmd, reader, _defaultMapper);
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
            }

            return result;
        }

        #endregion

        #region Fetch, FetchAsync : Single-Poco

        /// <inheritdoc/>
        public List<T> Fetch<T>()
            => Fetch<T>(string.Empty);

        /// <inheritdoc/>
        public List<T> Fetch<T>(Sql sql)
            => Fetch<T>(sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public List<T> Fetch<T>(string sql, params object[] args)
            => Query<T>(sql, args).ToList();

#if ASYNC
        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>()
            => FetchAsync<T>(CancellationToken.None, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(Sql sql)
            => FetchAsync<T>(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(string sql, params object[] args)
            => FetchAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken)
            => FetchAsync<T>(cancellationToken, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, Sql sql)
            => FetchAsync<T>(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => FetchAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CommandType commandType)
            => FetchAsync<T>(CancellationToken.None, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CommandType commandType, Sql sql)
            => FetchAsync<T>(CancellationToken.None, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CommandType commandType, string sql, params object[] args)
            => FetchAsync<T>(CancellationToken.None, commandType, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType)
            => FetchAsync<T>(cancellationToken, commandType, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql)
            => FetchAsync<T>(cancellationToken, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public async Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            var pocos = new List<T>();
            await QueryAsync<T>(p => pocos.Add(p), cancellationToken, commandType, sql, args).ConfigureAwait(false);
            return pocos;
        }
#endif

        #endregion

        #region Fetch : Multi-Poco

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2>(Sql sql)
            => Query<T1, T2>(sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3>(Sql sql)
            => Query<T1, T2, T3>(sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3, T4>(Sql sql)
            => Query<T1, T2, T3, T4>(sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3, T4, T5>(Sql sql)
            => Query<T1, T2, T3, T4, T5>(sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2>(string sql, params object[] args)
            => Query<T1, T2>(sql, args).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3>(string sql, params object[] args)
            => Query<T1, T2, T3>(sql, args).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args)
            => Query<T1, T2, T3, T4>(sql, args).ToList();

        /// <inheritdoc/>
        public List<T1> Fetch<T1, T2, T3, T4, T5>(string sql, params object[] args)
            => Query<T1, T2, T3, T4, T5>(sql, args).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
            => Query(cb, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
            => Query(cb, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
            => Query(cb, sql.SQL, sql.Arguments).ToList();

        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql)
            => Query(cb, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
            => Query(cb, sql, args).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
            => Query(cb, sql, args).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
            => Query(cb, sql, args).ToList();

        /// <inheritdoc/>
        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args)
            => Query(cb, sql, args).ToList();

        #endregion

        #region Fetch, FetchAsync : Paged SkipTake

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long itemsPerPage)
            => Fetch<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
            => SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
            => SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);

#if ASYNC
        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage)
            => FetchAsync<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql)
            => FetchAsync<T>(CancellationToken.None, page, itemsPerPage, sql);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args)
            => FetchAsync<T>(CancellationToken.None, page, itemsPerPage, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage)
            => FetchAsync<T>(cancellationToken, page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * itemsPerPage, itemsPerPage, sql, args);
#endif

        #endregion

        #region Page, PageAsync

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long itemsPerPage)
            => Page<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
            => Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out var sqlCount, out var sqlPage);
            return Page<T>(page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => Page<T>(page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);

#if ASYNC
        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sql)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, params object[] args)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sql, args);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sqlCount, countArgs, sqlPage, pageArgs);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage)
            => PageAsync<T>(cancellationToken, page, itemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql)
            => PageAsync<T>(cancellationToken, page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out var sqlCount, out var sqlPage);
            return PageAsync<T>(cancellationToken, page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => PageAsync<T>(cancellationToken, page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);
#endif

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T>
            {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = ExecuteScalar<long>(sqlCount, countArgs)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if (result.TotalItems % itemsPerPage != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            result.Items = Fetch<T>(sqlPage, pageArgs);

            return result;
        }

#if ASYNC
        /// <inheritdoc/>
        public async Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            var saveTimeout = OneTimeCommandTimeout;

            var result = new Page<T>
            {
                CurrentPage = page,
                ItemsPerPage = itemsPerPage,
                TotalItems = await ExecuteScalarAsync<long>(cancellationToken, sqlCount, countArgs).ConfigureAwait(false)
            };
            result.TotalPages = result.TotalItems / itemsPerPage;

            if (result.TotalItems % itemsPerPage != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            result.Items = await FetchAsync<T>(cancellationToken, sqlPage, pageArgs).ConfigureAwait(false);

            return result;
        }
#endif

        #endregion

        #region SkipTake, SkipTakeAsync

        /// <inheritdoc/>
        public List<T> SkipTake<T>(long skip, long take)
            => SkipTake<T>(skip, take, string.Empty);

        /// <inheritdoc/>
        public List<T> SkipTake<T>(long skip, long take, Sql sql)
            => SkipTake<T>(skip, take, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            BuildPageQueries<T>(skip, take, sql, ref args, out var sqlCount, out var sqlPage);
            return Fetch<T>(sqlPage, args);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take)
            => SkipTakeAsync<T>(CancellationToken.None, skip, take, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql)
            => SkipTakeAsync<T>(skip, take, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args)
            => SkipTakeAsync<T>(CancellationToken.None, skip, take, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take)
            => SkipTakeAsync<T>(cancellationToken, skip, take, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, Sql sql)
            => SkipTakeAsync<T>(cancellationToken, skip, take, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, string sql, params object[] args)
        {
            BuildPageQueries<T>(skip, take, sql, ref args, out var sqlCount, out var sqlPage);
            return FetchAsync<T>(cancellationToken, sqlPage, args);
        }
#endif

        #endregion

        /// <summary>
        /// Starting with a regular SELECT statement, derives the SQL statements required to query a DB for a page of records and the total number of records.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="skip">The number of rows to skip before the start of the page.</param>
        /// <param name="take">The number of rows in the page.</param>
        /// <param name="sql">The original SQL select statement.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <param name="sqlCount">Outputs the SQL statement to query for the total number of matching rows.</param>
        /// <param name="sqlPage">Outputs the SQL statement to retrieve a single page of matching rows.</param>
        /// <exception cref="Exception">Thrown when unable to parse the given <paramref name="sql"/> statement.</exception>
        protected virtual void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            SQLParts parts;
            if (!Provider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            sqlPage = _provider.BuildPageQuery(skip, take, parts, ref args);
            sqlCount = parts.SqlCount;
        }

        #region Exists, ExistsAsync

        /// <inheritdoc/>
        public bool Exists<T>(object primaryKey)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return Exists<T>($"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                primaryKey is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(primaryKey) : primaryKey);
        }

        /// <inheritdoc/>
        public bool Exists<T>(string sqlCondition, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sqlCondition.TrimStart().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart().Substring(5);

            return ExecuteScalar<int>(string.Format(_provider.GetExistsSql(), Provider.EscapeTableName(poco.TableName), sqlCondition), args) != 0;
        }

#if ASYNC
        public Task<bool> ExistsAsync<T>(object primaryKey)
            => ExistsAsync<T>(CancellationToken.None, primaryKey);

        public Task<bool> ExistsAsync<T>(string sqlCondition, params object[] args)
            => ExistsAsync<T>(CancellationToken.None, sqlCondition, args);

        public Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, object primaryKey)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return ExistsAsync<T>(cancellationToken, $"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                primaryKey is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(primaryKey) : primaryKey);
        }

        public async Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, string sqlCondition, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sqlCondition.TrimStart().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart().Substring(5);

            return await ExecuteScalarAsync<int>(cancellationToken,
                       string.Format(_provider.GetExistsSql(), Provider.EscapeTableName(poco.TableName), sqlCondition), args).ConfigureAwait(false) != 0;
        }
#endif

        #endregion

        #region Single, SingleAsync

        /// <inheritdoc/>
        public T Single<T>(object primaryKey)
            => Single<T>(GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc/>
        public T Single<T>(Sql sql)
            => Query<T>(sql).Single();

        /// <inheritdoc/>
        public T Single<T>(string sql, params object[] args)
            => Query<T>(sql, args).Single();

#if ASYNC
        /// <inheritdoc/>
        public Task<T> SingleAsync<T>(object primaryKey)
            => SingleAsync<T>(CancellationToken.None, primaryKey);

        /// <inheritdoc/>
        public Task<T> SingleAsync<T>(Sql sql)
            => SingleAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<T> SingleAsync<T>(string sql, params object[] args)
            => SingleAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken, object primaryKey)
            => SingleAsync<T>(cancellationToken, GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc/>
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken, Sql sql)
            => SingleAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).Single();
#endif

        #endregion

        #region SingleOrDefault, SingleOrDefaultAsync

        /// <inheritdoc/>
        public T SingleOrDefault<T>(object primaryKey)
            => SingleOrDefault<T>(GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc/>
        public T SingleOrDefault<T>(Sql sql)
            => Query<T>(sql).SingleOrDefault();

        /// <inheritdoc/>
        public T SingleOrDefault<T>(string sql, params object[] args)
            => Query<T>(sql, args).SingleOrDefault();

#if ASYNC
        /// <inheritdoc/>
        public Task<T> SingleOrDefaultAsync<T>(object primaryKey)
            => SingleOrDefaultAsync<T>(CancellationToken.None, primaryKey);

        /// <inheritdoc/>
        public Task<T> SingleOrDefaultAsync<T>(Sql sql)
            => SingleOrDefaultAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<T> SingleOrDefaultAsync<T>(string sql, params object[] args)
            => SingleOrDefaultAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, object primaryKey)
            => SingleOrDefaultAsync<T>(cancellationToken, GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc/>
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql)
            => SingleOrDefaultAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).SingleOrDefault();
#endif

        private Sql GenerateSingleByKeySql<T>(object primaryKey)
        {
            string pkName = _provider.EscapeSqlIdentifier(PocoData.ForType(typeof(T), _defaultMapper).TableInfo.PrimaryKey);
            var sql = $"WHERE {pkName} = @0";

            if (!EnableAutoSelect)
                // We're going to be nice and add the SELECT anyway
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return new Sql(sql, primaryKey);
        }

        #endregion

        #region First, FirstAsync

        /// <inheritdoc/>
        public T First<T>(Sql sql)
            => Query<T>(sql).First();

        /// <inheritdoc/>
        public T First<T>(string sql, params object[] args)
            => Query<T>(sql, args).First();

#if ASYNC
        /// <inheritdoc/>
        public Task<T> FirstAsync<T>(Sql sql)
            => FirstAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<T> FirstAsync<T>(string sql, params object[] args)
            => FirstAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<T> FirstAsync<T>(CancellationToken cancellationToken, Sql sql)
            => FirstAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).First();
#endif

        #endregion

        #region FirstOrDefault, FirstOrDefaultAsync

        /// <inheritdoc/>
        public T FirstOrDefault<T>(Sql sql)
            => Query<T>(sql).FirstOrDefault();

        /// <inheritdoc/>
        public T FirstOrDefault<T>(string sql, params object[] args)
            => Query<T>(sql, args).FirstOrDefault();

#if ASYNC
        /// <inheritdoc/>
        public Task<T> FirstOrDefaultAsync<T>(Sql sql)
            => FirstOrDefaultAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args)
            => FirstOrDefaultAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql)
            => FirstOrDefaultAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).FirstOrDefault();
#endif

        #endregion

        #region Insert, InsertAsync

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="poco"/> is null.</exception>
        public object Insert(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tableName"/> or <paramref name="poco"/> is null or empty.</exception>
        public object Insert(string tableName, object poco)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);

            return ExecuteInsert(tableName, pd == null ? null : pd.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        public object Insert(string tableName, string primaryKeyName, object poco)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var t = poco.GetType();
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            var autoIncrement = pd == null || pd.TableInfo.AutoIncrement || t.Name.Contains("AnonymousType") &&
                                !t.GetProperties().Any(p => p.Name.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase));

            return ExecuteInsert(tableName, primaryKeyName, autoIncrement, poco);
        }

        /// <inheritdoc/>
        public object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return ExecuteInsert(tableName, primaryKeyName, autoIncrement, poco);
        }

#if ASYNC
        public Task<object> InsertAsync(object poco)
            => InsertAsync(CancellationToken.None, poco);

        public Task<object> InsertAsync(string tableName, object poco)
            => InsertAsync(CancellationToken.None, tableName, poco);

        public Task<object> InsertAsync(string tableName, string primaryKeyName, object poco)
            => InsertAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        public Task<object> InsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco)
            => InsertAsync(CancellationToken.None, tableName, primaryKeyName, autoIncrement, poco);

        public Task<object> InsertAsync(CancellationToken cancellationToken, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsertAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, object poco)
        {
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsertAsync(cancellationToken, tableName, pd?.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
        {
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (primaryKeyName == null)
                throw new ArgumentNullException(nameof(primaryKeyName));
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var t = poco.GetType();
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            var autoIncrement = pd == null || pd.TableInfo.AutoIncrement || t.Name.Contains("AnonymousType") &&
                                !t.GetProperties().Any(p => p.Name.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase));

            return ExecuteInsertAsync(cancellationToken, tableName, primaryKeyName, autoIncrement, poco);
        }

        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (primaryKeyName == null)
                throw new ArgumentNullException(nameof(primaryKeyName));
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return ExecuteInsertAsync(cancellationToken, tableName, primaryKeyName, autoIncrement, poco);
        }
#endif

        private void PrepareExecuteInsert(string tableName, string primaryKeyName, bool autoIncrement, object poco, PocoData pd, List<string> names, List<string> values, IDbCommand cmd)
        {
            var index = 0;
            foreach (var i in pd.Columns)
            {
                // Don't insert result columns
                if (i.Value.ResultColumn)
                    continue;

                // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                if (autoIncrement && primaryKeyName != null && string.Compare(i.Key, primaryKeyName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // Setup auto increment expression
                    var autoIncExpression = _provider.GetAutoIncrementExpression(pd.TableInfo);
                    if (autoIncExpression != null)
                    {
                        names.Add(_provider.EscapeSqlIdentifier(i.Key));
                        values.Add(autoIncExpression);
                    }

                    continue;
                }

                names.Add(_provider.EscapeSqlIdentifier(i.Key));
                values.Add(string.Format(i.Value.InsertTemplate ?? "{0}{1}", _paramPrefix, index++));
                AddParam(cmd, i.Value.GetValue(poco), i.Value);
            }

            var outputClause = string.Empty;
            if (autoIncrement)
                outputClause = _provider.GetInsertOutputClause(primaryKeyName);

            cmd.CommandText =
                $"INSERT INTO {_provider.EscapeTableName(tableName)} ({string.Join(",", names.ToArray())}){outputClause} VALUES ({string.Join(",", values.ToArray())})";
        }

        private object ExecuteInsert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, string.Empty))
                    {
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        var names = new List<string>();
                        var values = new List<string>();

                        PrepareExecuteInsert(tableName, primaryKeyName, autoIncrement, poco, pd, names, values, cmd);

                        if (!autoIncrement)
                        {
                            ExecuteNonQueryHelper(cmd);

                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out var pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        var id = _provider.ExecuteInsert(this, cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType().Name.Contains("AnonymousType"))
                            if (pd.Columns.TryGetValue(primaryKeyName, out var pc))
                                pc.SetValue(poco, pc.ChangeType(id));

                        return id;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return null;
            }
        }

#if ASYNC
        private async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, string.Empty))
                    {
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        var names = new List<string>();
                        var values = new List<string>();

                        PrepareExecuteInsert(tableName, primaryKeyName, autoIncrement, poco, pd, names, values, cmd);

                        if (!autoIncrement)
                        {
                            await ExecuteNonQueryHelperAsync(cancellationToken, cmd).ConfigureAwait(false);

                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out var pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        var id = await _provider.ExecuteInsertAsync(cancellationToken, this, cmd, primaryKeyName).ConfigureAwait(false);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType().Name.Contains("AnonymousType"))
                            if (pd.Columns.TryGetValue(primaryKeyName, out var pc))
                                pc.SetValue(poco, pc.ChangeType(id));

                        return id;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return null;
            }
        }
#endif

        #endregion

        #region Update, UpdateAsync

        /// <inheritdoc/>
        public int Update(object poco)
            => Update(poco, null, null);

        /// <inheritdoc/>
        public int Update(object poco, IEnumerable<string> columns)
            => Update(poco, null, columns);

        /// <inheritdoc/>
        public int Update(object poco, object primaryKeyValue)
            => Update(poco, primaryKeyValue, null);

        /// <inheritdoc/>
        public int Update(object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (columns?.Any() == false)
                return 0;

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteUpdate(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        /// <inheritdoc/>
        public int Update(string tableName, string primaryKeyName, object poco)
            => Update(tableName, primaryKeyName, poco, null);

        /// <inheritdoc/>
        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => Update(tableName, primaryKeyName, poco, null, columns);

        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => Update(tableName, primaryKeyName, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref name="poco"/> is null or empty.</exception>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (columns?.Any() == false)
                return 0;

            return ExecuteUpdate(tableName, primaryKeyName, poco, primaryKeyValue, columns);
        }

        /// <inheritdoc/>
        public int Update<T>(Sql sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(new Sql($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        public int Update<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)} {sql}", args);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<int> UpdateAsync(object poco)
            => UpdateAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(object poco, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, poco, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(object poco, object primaryKeyValue)
            => UpdateAsync(CancellationToken.None, poco, primaryKeyValue);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(object poco, object primaryKeyValue, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, poco, primaryKeyValue, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, primaryKeyValue);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, primaryKeyValue, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync<T>(Sql sql)
            => UpdateAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<int> UpdateAsync<T>(string sql, params object[] args)
            => UpdateAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco)
            => UpdateAsync(cancellationToken, poco, null, null);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, IEnumerable<string> columns)
            => UpdateAsync(cancellationToken, poco, null, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue)
            => UpdateAsync(cancellationToken, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (columns?.Any() == false)
                return Task.FromResult(0);

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteUpdateAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, null);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, null, columns);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (columns?.Any() == false)
                return Task.FromResult(0);

            return ExecuteUpdateAsync(cancellationToken, tableName, primaryKeyName, poco, primaryKeyValue, columns);
        }

        /// <inheritdoc/>
        public Task<int> UpdateAsync<T>(CancellationToken cancellationToken, Sql sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(cancellationToken, new Sql($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        public Task<int> UpdateAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(cancellationToken, $"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)} {sql}", args);
        }
#endif

        private void PreExecuteUpdate(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns, IDbCommand cmd)
        {
            var sb = new StringBuilder();
            var index = 0;
            var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
            if (columns == null)
            {
                foreach (var i in pd.Columns)
                {
                    // Don't update the primary key, but grab the value if we don't have it
                    if (string.Compare(i.Key, primaryKeyName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (primaryKeyValue == null)
                            primaryKeyValue = i.Value.GetValue(poco);
                        continue;
                    }

                    // Dont update result only columns
                    if (i.Value.ResultColumn)
                        continue;

                    // Build the sql
                    if (index > 0)
                        sb.Append(", ");
                    sb.AppendFormat(i.Value.UpdateTemplate ?? "{0} = {1}{2}", _provider.EscapeSqlIdentifier(i.Key), _paramPrefix, index++);

                    // Store the parameter in the command
                    AddParam(cmd, i.Value.GetValue(poco), i.Value);
                }
            }
            else
            {
                foreach (var colname in columns)
                {
                    var pc = pd.Columns[colname];

                    // Build the sql
                    if (index > 0)
                        sb.Append(", ");
                    sb.AppendFormat(pc.UpdateTemplate ?? "{0} = {1}{2}", _provider.EscapeSqlIdentifier(colname), _paramPrefix, index++);

                    // Store the parameter in the command
                    AddParam(cmd, pc.GetValue(poco), pc);
                }

                // Grab primary key value
                if (primaryKeyValue == null)
                {
                    var pc = pd.Columns[primaryKeyName];
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Find the property info for the primary key
            PocoColumn col = null;
            if (primaryKeyName != null)
            {
                if (!pd.Columns.TryGetValue(primaryKeyName, out col))
                {
                    var pkpi = new { Id = primaryKeyValue }.GetType().GetProperty("Id");
                    col = new PocoColumn() { PropertyInfo = pkpi };
                }
            }

            cmd.CommandText =
                $"UPDATE {_provider.EscapeTableName(tableName)} SET {sb} WHERE {_provider.EscapeSqlIdentifier(primaryKeyName)} = {_paramPrefix}{index++}";
            AddParam(cmd, primaryKeyValue, col);
        }

        private int ExecuteUpdate(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, string.Empty))
                    {
                        PreExecuteUpdate(tableName, primaryKeyName, poco, primaryKeyValue, columns, cmd);
                        return ExecuteNonQueryHelper(cmd);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return -1;
            }
        }

#if ASYNC
        private async Task<int> ExecuteUpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, string.Empty))
                    {
                        PreExecuteUpdate(tableName, primaryKeyName, poco, primaryKeyValue, columns, cmd);
                        return await ExecuteNonQueryHelperAsync(cancellationToken, cmd).ConfigureAwait(false);
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                if (OnException(x))
                    throw;
                return -1;
            }
        }
#endif

        #endregion

        #region Delete, DeleteAsync

        /// <inheritdoc/>
        public int Delete(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <inheritdoc/>
        public int Delete(string tableName, string primaryKeyName, object poco)
            => Delete(tableName, primaryKeyName, poco, null);

        /// <inheritdoc/>
        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            if (primaryKeyValue == null)
            {
                var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                if (pd.Columns.TryGetValue(primaryKeyName, out var pc))
                    primaryKeyValue = pc.GetValue(poco);
            }

            var sql = $"DELETE FROM {_provider.EscapeTableName(tableName)} WHERE {_provider.EscapeSqlIdentifier(primaryKeyName)}=@0";
            return Execute(sql, primaryKeyValue);
        }

        /// <inheritdoc/>
        public int Delete<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType"))
            {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException($"Anonymous type does not contain an id for PK column `{pd.TableInfo.PrimaryKey}`.");

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <inheritdoc/>
        public int Delete<T>(Sql sql)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(new Sql($"DELETE FROM {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        public int Delete<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute($"DELETE FROM {_provider.EscapeTableName(pd.TableInfo.TableName)} {sql}", args);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<int> DeleteAsync(object poco)
            => DeleteAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        public Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco)
            => DeleteAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        public Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => DeleteAsync(CancellationToken.None, tableName, primaryKeyName, poco, primaryKeyValue);

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(object pocoOrPrimaryKey)
            => DeleteAsync<T>(CancellationToken.None, pocoOrPrimaryKey);

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(Sql sql)
            => DeleteAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(string sql, params object[] args)
            => DeleteAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<int> DeleteAsync(CancellationToken cancellationToken, object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return DeleteAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <inheritdoc/>
        public Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
            => DeleteAsync(cancellationToken, tableName, primaryKeyName, poco, null);

        /// <inheritdoc/>
        public Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            if (primaryKeyValue == null)
            {
                var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                if (pd.Columns.TryGetValue(primaryKeyName, out var pc))
                    primaryKeyValue = pc.GetValue(poco);
            }

            var sql = $"DELETE FROM {_provider.EscapeTableName(tableName)} WHERE {_provider.EscapeSqlIdentifier(primaryKeyName)}=@0";
            return ExecuteAsync(cancellationToken, sql, primaryKeyValue);
        }

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return DeleteAsync(cancellationToken, pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType"))
            {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException($"Anonymous type does not contain an id for PK column `{pd.TableInfo.PrimaryKey}`.");

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return DeleteAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(CancellationToken cancellationToken, Sql sql)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(cancellationToken, new Sql($"DELETE FROM {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        public Task<int> DeleteAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(cancellationToken, $"DELETE FROM {_provider.EscapeTableName(pd.TableInfo.TableName)} {sql}", args);
        }
#endif

        #endregion

        #region IsNew

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="poco"/> is null.</exception>
        public bool IsNew(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return IsNew(pd.TableInfo.PrimaryKey, pd, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="poco"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="primaryKeyName"/> is null or empty.</exception>
        public bool IsNew(string primaryKeyName, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            return IsNew(primaryKeyName, PocoData.ForObject(poco, primaryKeyName, _defaultMapper), poco);
        }

        /// <inheritdoc cref="IAlterPoco.IsNew(string, object)" />
        /// <param name="primaryKeyName">The name of the primary key column.</param>
        /// <param name="pd">A PocoData object for the object instance.</param>
        /// <param name="poco">The object instance whose "newness" is to be tested.</param>
        /// <exception cref="InvalidOperationException">Thrown when the table represented by the given <paramref name="poco"/> has no auto-incrementing primary key column.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="poco"/> doesn't have a property matching the primary key column name.</exception>
        protected virtual bool IsNew(string primaryKeyName, PocoData pd, object poco)
        {
            if (string.IsNullOrEmpty(primaryKeyName) || poco is ExpandoObject)
                throw new InvalidOperationException("IsNew() and Save() are only supported on tables with identity (inc auto-increment) primary key columns");

            object pk;
            PocoColumn pc;
            PropertyInfo pi;
            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
            {
                pk = pc.GetValue(poco);
                pi = pc.PropertyInfo;
            }
            else
            {
                pi = poco.GetType().GetProperty(primaryKeyName);
                if (pi == null)
                    throw new ArgumentException(string.Format("The object doesn't have a property matching the primary key column name '{0}'", primaryKeyName));
                pk = pi.GetValue(poco, null);
            }

            var type = pk != null ? pk.GetType() : pi.PropertyType;

            if (type == typeof(string))
                return string.IsNullOrEmpty((string)pk);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) || !type.IsValueType)
                return pk == null;
            if (!pi.PropertyType.IsValueType)
                return pk == null;
            if (type == typeof(long))
                return (long) pk == default(long);
            if (type == typeof(int))
                return (int) pk == default(int);
            if (type == typeof(Guid))
                return (Guid) pk == default(Guid);
            if (type == typeof(ulong))
                return (ulong) pk == default(ulong);
            if (type == typeof(uint))
                return (uint) pk == default(uint);
            if (type == typeof(short))
                return (short) pk == default(short);
            if (type == typeof(ushort))
                return (ushort) pk == default(ushort);
            if (type == typeof(decimal))
                return (decimal) pk == default(decimal);

            // Create a default instance and compare
            return pk == Activator.CreateInstance(pk.GetType());
        }

        #endregion

        #region Save, SaveAsync

        /// <inheritdoc/>
        public void Save(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            Save(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <inheritdoc/>
        public void Save(string tableName, string primaryKeyName, object poco)
        {
            if (IsNew(primaryKeyName, poco))
                Insert(tableName, primaryKeyName, true, poco);
            else
                Update(tableName, primaryKeyName, poco);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task SaveAsync(object poco)
            => SaveAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        public Task SaveAsync(string tableName, string primaryKeyName, object poco)
            => SaveAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        public Task SaveAsync(CancellationToken cancellationToken, object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return SaveAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <inheritdoc/>
        public Task SaveAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
        {
            if (IsNew(primaryKeyName, poco))
                return InsertAsync(cancellationToken, tableName, primaryKeyName, true, poco);

            return UpdateAsync(cancellationToken, tableName, primaryKeyName, poco);
        }
#endif

        #endregion

        #region StoredProcedures, StoredProceduresAsync

        /// <inheritdoc/>
        public int ExecuteNonQueryProc(string storedProcedureName, params object[] args)
            => ExecuteInternal(CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public T ExecuteScalarProc<T>(string storedProcedureName, params object[] args)
            => ExecuteScalarInternal<T>(CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public IEnumerable<T> QueryProc<T>(string storedProcedureName, params object[] args)
            => ExecuteReader<T>(CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public List<T> FetchProc<T>(string storedProcedureName, params object[] args)
            => QueryProc<T>(storedProcedureName, args).ToList();

#if ASYNC
        /// <inheritdoc/>
        public Task<int> ExecuteNonQueryProcAsync(string storedProcedureName, params object[] args)
            => ExecuteNonQueryProcAsync(CancellationToken.None, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<T> ExecuteScalarProcAsync<T>(string storedProcedureName, params object[] args)
            => ExecuteScalarProcAsync<T>(CancellationToken.None, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryProcAsync<T>(string storedProcedureName, params object[] args)
            => QueryProcAsync<T>(CancellationToken.None, storedProcedureName, args);

        /// <inheritdoc/>
        public Task QueryProcAsync<T>(Action<T> receivePocoCallback, string storedProcedureName, params object[] args)
            => QueryProcAsync(receivePocoCallback, CancellationToken.None, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchProcAsync<T>(string storedProcedureName, params object[] args)
            => FetchProcAsync<T>(CancellationToken.None, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<int> ExecuteNonQueryProcAsync(CancellationToken cancellationToken, string storedProcedureName, params object[] args)
            => ExecuteInternalAsync(cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<T> ExecuteScalarProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args)
            => ExecuteScalarInternalAsync<T>(cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public Task<IAsyncReader<T>> QueryProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args)
            => ExecuteReaderAsync<T>(cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public Task QueryProcAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string storedProcedureName, params object[] args)
            => ExecuteReaderAsync(receivePocoCallback, cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);

        /// <inheritdoc/>
        public async Task<List<T>> FetchProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args)
        {
            var pocos = new List<T>();
            await ExecuteReaderAsync<T>(p => pocos.Add(p), cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);
            return pocos;
        }
#endif

        #endregion

        #region Command & Parameter Creation

        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
            => CreateCommand(connection, CommandType.Text, sql, args);

        public IDbCommand CreateCommand(IDbConnection connection, CommandType commandType, string sql, params object[] args)
        {
            var cmd = connection.CreateCommand();

            try
            {
                cmd.CommandType = commandType;
                cmd.Transaction = _transaction;

                switch (commandType)
                {
                    case CommandType.Text:
                        // Perform named argument replacements
                        if (EnableNamedParams)
                        {
                            var newArgs = new List<object>();
                            sql = ParametersHelper.ProcessQueryParams(sql, args, newArgs);
                            args = newArgs.ToArray();
                        }

                        // Perform parameter prefix replacements
                        if (_paramPrefix != "@")
                            sql = sql.ReplaceParamPrefix(_paramPrefix);
                        sql = sql.Replace("@@", "@"); // <- double @@ escapes a single @
                        break;
                    case CommandType.StoredProcedure:
                        args = ParametersHelper.ProcessStoredProcParams(cmd, args, SetParameterProperties);
                        break;
                    case CommandType.TableDirect:
                        break;
                }

                cmd.CommandText = sql;

                foreach (var item in args)
                    AddParam(cmd, item, null);

                return cmd;
            }
            catch
            {
                cmd.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates an IDbDataParameter with default values.
        /// </summary>
        /// <returns>The IDbDataParameter.</returns>
        public IDbDataParameter CreateParameter()
            => _factory.CreateParameter();

        /// <summary>
        /// Creates an IDbDataParameter with the given name and value.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The IDbDataParameter.</returns>
        public IDbDataParameter CreateParameter(string name, object value)
            => CreateParameter(name, value, ParameterDirection.Input);

        /// <summary>
        /// Creates an IDbDataParameter with the given name and direction.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <returns>The IDbDataParameter.</returns>
        public IDbDataParameter CreateParameter(string name, ParameterDirection direction)
            => CreateParameter(name, null, direction);

        /// <summary>
        /// Create an IDbParameter with the given ParameterName, Value, and Direction.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="direction">The parameter direction.</param>
        /// <returns>The IDbDataParameter.</returns>
        public IDbDataParameter CreateParameter(string name, object value, ParameterDirection direction)
        {
            var result = CreateParameter();
            result.ParameterName = name;
            result.Value = value;
            result.Direction = direction;
            return result;
        }

        private void SetParameterProperties(IDbDataParameter p, object value, PocoColumn pc)
        {
            // Assign the parameter value
            if (value == null)
            {
                p.Value = DBNull.Value;

                if (pc?.PropertyInfo.PropertyType.Name == "Byte[]")
                    p.DbType = DbType.Binary;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = _provider.MapParameterValue(value);

                var t = value.GetType();

                if (t == typeof(string) && pc?.ForceToAnsiString == true)
                {
                    t = typeof(AnsiString);
                    value = value.ToAnsiString();
                }
                if (t == typeof(DateTime) && pc?.ForceToDateTime2 == true)
                {
                    t = typeof(DateTime2);
                    value = ((DateTime)value).ToDateTime2();
                }

                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = Convert.ChangeType(value, ((Enum)value).GetTypeCode());
                }
                else if (t == typeof(Guid) && !_provider.HasNativeGuidSupport)
                {
                    p.Value = value.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);

                    p.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                }
                else if (t == typeof(AnsiString))
                {
                    var asValue = (value as AnsiString).Value;
                    if (asValue == null)
                    {
                        p.Size = 0;
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Size = Math.Max(asValue.Length + 1, 4000);
                        p.Value = asValue;
                    }
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.DbType = DbType.AnsiString;
                }
                else if (t == typeof(DateTime2))
                {
                    var dt2Value = (value as DateTime2)?.Value;
                    p.Value = dt2Value ?? (object)DBNull.Value;
                    p.DbType = DbType.DateTime2;
                }
                else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else if (t == typeof(byte[]))
                {
                    p.Value = value;
                    p.DbType = DbType.Binary;
                }
                else
                {
                    p.Value = value;
                }
            }
        }

        /// <summary>
        /// Adds a parameter to a DB command.
        /// </summary>
        /// <param name="cmd">A reference to the IDbCommand to which the parameter is to be added.</param>
        /// <param name="value">The value to assign to the parameter.</param>
        /// <param name="pc">Optional, a reference to the property info of the POCO property from which the value is coming.</param>
        private void AddParam(IDbCommand cmd, object value, PocoColumn pc)
        {
            // Convert value to from poco type to db type
            if (pc != null)
            {
                var mapper = Mappers.GetMapper(pc.PropertyInfo.DeclaringType, _defaultMapper);
                var fn = mapper.GetToDbConverter(pc.PropertyInfo);
                if (fn != null)
                    value = fn(value);
            }

            // Support passed in parameters
            if (value is IDbDataParameter idbParam)
            {
                if (cmd.CommandType == CommandType.Text)
                    idbParam.ParameterName = cmd.Parameters.Count.EnsureParamPrefix(_paramPrefix);
                else if (idbParam.ParameterName?.StartsWith(_paramPrefix) != true)
                    idbParam.ParameterName = idbParam.ParameterName.EnsureParamPrefix(_paramPrefix);

                cmd.Parameters.Add(idbParam);
            }
            else
            {
                var p = cmd.CreateParameter();
                p.ParameterName = cmd.Parameters.Count.EnsureParamPrefix(_paramPrefix);
                SetParameterProperties(p, value, pc);

                cmd.Parameters.Add(p);
            }
        }

        #endregion

        #region Execute Command Helpers

        internal protected IDataReader ExecuteReaderHelper(IDbCommand cmd)
        {
            return (IDataReader)CommandHelper(cmd, c => c.ExecuteReader());
        }

        internal protected int ExecuteNonQueryHelper(IDbCommand cmd)
        {
            return (int)CommandHelper(cmd, c => c.ExecuteNonQuery());
        }

        internal protected object ExecuteScalarHelper(IDbCommand cmd)
        {
            return CommandHelper(cmd, c => c.ExecuteScalar());
        }

        private object CommandHelper(IDbCommand cmd, Func<IDbCommand, object> cmdFunc)
        {
            DoPreExecute(cmd);
            var result = cmdFunc(cmd);
            OnExecutedCommand(cmd);
            return result;
        }

#if ASYNC
        internal protected async Task<IDataReader> ExecuteReaderHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
            {
                var task = CommandHelper(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteReaderAsync(t).ConfigureAwait(false));
                return (IDataReader)await task.ConfigureAwait(false);
            }
            else
                return ExecuteReaderHelper(cmd);
        }

        internal protected async Task<int> ExecuteNonQueryHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
            {
                var task = CommandHelper(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteNonQueryAsync(t).ConfigureAwait(false));
                return (int)await task.ConfigureAwait(false);
            }
            else
                return ExecuteNonQueryHelper(cmd);
        }

        internal protected Task<object> ExecuteScalarHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
                return CommandHelper(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
            else
                return Task.FromResult(ExecuteScalarHelper(cmd));
        }

        private async Task<object> CommandHelper(CancellationToken cancellationToken, DbCommand cmd,
            Func<CancellationToken, DbCommand, Task<object>> cmdFunc)
        {
            DoPreExecute(cmd);
            var result = await cmdFunc(cancellationToken, cmd).ConfigureAwait(false);
            OnExecutedCommand(cmd);
            return result;
        }
#endif

        #endregion

        internal void DoPreExecute(IDbCommand cmd)
        {
            if (CommandTimeout > 0 || OneTimeCommandTimeout > 0)
            {
                cmd.CommandTimeout = OneTimeCommandTimeout > 0 ? OneTimeCommandTimeout : CommandTimeout;
                OneTimeCommandTimeout = 0;
            }

            _provider.PreExecute(cmd);
            OnExecutingCommand(cmd);

            _lastSql = cmd.CommandText;
            _lastArgs = cmd.Parameters.Cast<IDataParameter>().Select(parameter => parameter.Value).ToArray();
        }

        #region Last Command, Format Command

        /// <summary>
        /// Gets the SQL used for the most recently executed statement.
        /// </summary>
        public string LastSQL => _lastSql;

        /// <summary>
        /// Gets the arguments used for the most recently executed statement.
        /// </summary>
        public object[] LastArgs => _lastArgs;

        /// <summary>
        /// Gets a formatted string describing the last executed SQL statement and its argument values.
        /// </summary>
        public string LastCommand => FormatCommand(_lastSql, _lastArgs);

        /// <summary>
        /// Formats the contents of a DB command for display.
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand"/>command.</param>
        /// <returns>The formatted command.</returns>
        public string FormatCommand(IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        /// <summary>
        /// Formats an SQL statement and its arguments for display.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The formatted SQL statement.</returns>
        public string FormatCommand(string sql, object[] args)
        {
            var sb = new StringBuilder();
            if (sql == null)
                return "";
            sb.Append(sql);
            if (args != null && args.Length > 0)
            {
                sb.Append("\n");
                for (int i = 0; i < args.Length; i++)
                {
                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i].GetType().Name, args[i]);
                }

                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        #endregion

        #region Configuration Properties

        /// <summary>
        /// Gets the default mapper.
        /// </summary>
        public IMapper DefaultMapper => _defaultMapper;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Gets the loaded database provider.
        /// </summary>
        public IProvider Provider => _provider;

        /// <summary>
        /// Gets or sets the transaction isolation level.
        /// </summary>
        /// <value>If <see langword="null"/>, the underlying provider's default isolation level is used.</value>
        public IsolationLevel? IsolationLevel
        {
            get => _isolationLevel;
            set
            {
                if (_transaction != null)
                    throw new InvalidOperationException("Isolation level can't be changed during a transaction.");

                _isolationLevel = value;
            }
        }

        /// <summary>
        /// When set to <see langword="true"/> the first opened connection is kept alive until <see cref="CloseSharedConnection" /> or <see cref="Dispose" /> is called.
        /// </summary>
        /// <seealso cref="OpenSharedConnection" />
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// When set to <see langword="true"/>, PetaPoco will automatically create the "SELECT columns" part of any query that looks like it needs it.
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        /// When set to <see langword="true"/>, parameters can be named <c>?myparam</c> and populated from properties of the passed-in argument values.
        /// </summary>
        public bool EnableNamedParams { get; set; }

        /// <summary>
        /// Gets or sets the timeout value for all SQL statements.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        public int OneTimeCommandTimeout { get; set; }

        #endregion

        #region Events

        /// <inheritdoc/>
        public event EventHandler<DbTransactionEventArgs> TransactionStarted;

        /// <inheritdoc/>
        public event EventHandler<DbTransactionEventArgs> TransactionEnding;

        /// <inheritdoc/>
        public event EventHandler<DbCommandEventArgs> CommandExecuting;

        /// <inheritdoc/>
        public event EventHandler<DbCommandEventArgs> CommandExecuted;

        /// <inheritdoc/>
        public event EventHandler<DbConnectionEventArgs> ConnectionOpening;

        /// <inheritdoc/>
        public event EventHandler<DbConnectionEventArgs> ConnectionOpened;

        /// <inheritdoc/>
        public event EventHandler<DbConnectionEventArgs> ConnectionClosing;

        /// <inheritdoc/>
        public event EventHandler<ExceptionEventArgs> ExceptionThrown;

        #endregion
    }

    /// <inheritdoc/>
    /// <typeparam name="TDatabaseProvider">The provider type, which must implement the <see cref="IProvider"/> interface.</typeparam>
    public class Database<TDatabaseProvider> : Database where TDatabaseProvider : IProvider
    {
        /// <summary>
        /// Constructs an instance using a supplied connection string and provider type.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString" /> is null or empty.</exception>
        public Database(string connectionString, IMapper defaultMapper = null)
            : base(connectionString, typeof(TDatabaseProvider).Name, defaultMapper)
        {
        }
    }
}
