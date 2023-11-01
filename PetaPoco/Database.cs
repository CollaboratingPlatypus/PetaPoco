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
    /// <summary>
    /// Represents the core functionality and implementation of PetaPoco.
    /// </summary>
    public class Database : IDatabase
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
        /// Constructs an instance with default values using the first connection string found in the app/web configuration file.
        /// </summary>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="InvalidOperationException">No connection strings are registered.</exception>
        public Database(IMapper defaultMapper = null)
        {
            if (ConfigurationManager.ConnectionStrings.Count == 0)
                throw new InvalidOperationException("One or more connection strings must be registered to use the no-parameter constructor");

            var entry = ConfigurationManager.ConnectionStrings[0];
            _connectionString = entry.ConnectionString;
            InitialiseFromEntry(entry, defaultMapper);
        }

        /// <summary>
        /// Constructs an instance with the specified connection string name. The connection string and database provider will be read from
        /// the app or web configuration file.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="connectionStringName">The name of the connection string to locate.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException"><paramref name="connectionStringName"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">A connection string cannot be found.</exception>
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
        /// Constructs an instance with the specified IDbConnection.
        /// </summary>
        /// <remarks>
        /// The supplied IDbConnection will not be closed and disposed of by PetaPoco - that remains the responsibility of the caller.
        /// </remarks>
        /// <param name="connection">The database connection.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> is null or empty.</exception>
        public Database(IDbConnection connection, IMapper defaultMapper = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            SetupFromConnection(connection);
            Initialise(DatabaseProvider.Resolve(_sharedConnection.GetType(), false, _connectionString), defaultMapper);
        }

        /// <summary>
        /// Constructs an instance with the specified connection string and database provider name.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">The database provider name.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException"><paramref name="connectionString"/> or <paramref name="providerName"/> is null or
        /// empty.</exception>
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
        /// Constructs an instance with the specified connection string and DbProviderFactory.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="factory">The database provider factory to use for database connections.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException"><paramref name="connectionString"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is null.</exception>
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
        /// Constructs an instance with the specified connection string and IProvider.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically close and dispose of any connections it creates.
        /// </remarks>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="provider">The database provider.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <exception cref="ArgumentException"><paramref name="connectionString"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
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
        /// Constructs an instance with the configured settings from the specified IDatabaseBuildConfiguration object.
        /// </summary>
        /// <param name="configuration">The build configuration instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A connection string cannot be found or is not configured, or unable to locate a
        /// provider.</exception>
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
                            throw new InvalidOperationException($"Cannot find a connection string with the name '{connectionStringName}'");
                    }
                    else
                    {
                        if (ConfigurationManager.ConnectionStrings.Count == 0)
                            throw new InvalidOperationException("One or more connection strings must be registered when not providing a connection string");

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

        #region Connection Management (IConnection implementation)

        /// <inheritdoc/>
        public IDbConnection Connection => _sharedConnection;

        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public Task OpenSharedConnectionAsync()
            => OpenSharedConnectionAsync(CancellationToken.None);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <inheritdoc cref="IConnection.OpenSharedConnectionAsync(CancellationToken)"/>
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

        /// <inheritdoc/>
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
        /// Releases the shared connection.
        /// </summary>
        /// <remarks>
        /// Implicitly called when the <see cref="Database"/> instance goes out of scope at the end of a <c>using</c> block, calling <see
        /// cref="CloseSharedConnection"/> to ensure the connection is properly closed.
        /// </remarks>
        public void Dispose()
        {
            // Automatically close one open connection reference
            // (Works with KeepConnectionAlive and manually opening a shared connection)
            CloseSharedConnection();
        }

        #endregion

        #region Transaction Management (ITransactionAccessor, IDatabase implementation)

        /// <inheritdoc/>
        IDbTransaction ITransactionAccessor.Transaction => _transaction;

        /// <inheritdoc/>
        /// <remarks>
        /// This method facilitates proper transaction lifetime management, especially when nested. Transactions can be nested but they must
        /// all include a call to <see cref="CompleteTransaction"/> <b>prior to exiting the scope</b>, otherwise the entire transaction is
        /// aborted.
        /// </remarks>
        /// <example>
        /// <para>A basic example of using transactional scopes (part pseudocode) is shown below:</para>
        /// <code language="cs" title="Transaction Scopes">
        /// <![CDATA[
        /// void DoStuff()
        /// {
        ///     using (var tx = db.GetTransaction()) // Starts transaction
        ///     {
        ///         db.Update(/*...*/); // Do stuff
        ///         if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        ///             DoDoubleStuff(); // Nested transaction scope
        ///         tx.Complete(); // Mark the transaction as complete
        ///     }
        /// }
        /// void DoDoubleStuff()
        /// {
        ///     using var tx = db.GetTransaction(); // Continues transaction if we're nested
        ///     db.Update(/*...*/); // Do boss's stuff, too
        ///     tx.Complete(); // Mark transaction as complete before we exit scope
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>An <see cref="ITransaction"/> reference that must be <see cref="CompleteTransaction">completed</see> or <see
        /// cref="Transaction.Dispose">disposed</see>.</returns>
        public ITransaction GetTransaction()
            => new Transaction(this);

        /// <summary>
        /// Called immediately after opening a transaction, and invokes the <see cref="IDatabase.TransactionStarted"/> event.
        /// </summary>
        public virtual void OnBeginTransaction()
        {
            TransactionStarted?.Invoke(this, new DbTransactionEventArgs(_transaction));
        }

        /// <summary>
        /// Called immediately before closing a transaction, and invokes the <see cref="IDatabase.TransactionEnding"/> event.
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
        /// <remarks>
        /// Called automatically by <see cref="Transaction.Dispose"/> if the transaction wasn't completed.
        /// </remarks>
        public void AbortTransaction()
        {
            _transactionCancelled = true;
            CompleteTransaction();
        }

#if ASYNC
        /// <inheritdoc cref="AbortTransaction"/>
        public Task AbortTransactionAsync()
        {
            _transactionCancelled = true;
            return CompleteTransactionAsync();
        }
#endif

        /// <inheritdoc/>
        /// <remarks>
        /// Not calling complete will cause the transaction to rollback on <see cref="Transaction.Dispose"/>.
        /// </remarks>
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

#if ASYNC
        /// <inheritdoc cref="CompleteTransaction"/>
        public async Task CompleteTransactionAsync()
        {
            if ((--_transactionDepth) == 0)
                await CleanupTransactionAsync().ConfigureAwait(false);
        }
#endif

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

        // TODO: Not in IDatabase interface: `OnException(Exception)`, to allow GridReader's DI impl to accept IDatabase (IAsyncReader is able to correctly use the interface for it's IOC because it doesn't require the OnException))

        /// <summary>
        /// Called if an exception is thrown during a database operation, and invokes the <see cref="IDatabase.ExceptionThrown"/> event.
        /// </summary>
        /// <param name="ex">The caught exception.</param>
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
        /// Called immediately after a database connection is opened, and invokes the <see cref="IDatabase.ConnectionOpened"/> event.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of opened connections, or to provide a proxy IDbConnection.
        /// </remarks>
        /// <param name="connection">The opened connection.</param>
        /// <returns>The same or a replacement IDbConnection.</returns>
        public virtual IDbConnection OnConnectionOpened(IDbConnection connection)
        {
            var args = new DbConnectionEventArgs(connection);
            ConnectionOpened?.Invoke(this, args);
            return args.Connection;
        }

        /// <summary>
        /// Called immediately before a database connection is opened, and invokes the <see cref="IDatabase.ConnectionOpening"/> event.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of opening connections, or to provide a proxy IDbConnection.
        /// </remarks>
        /// <param name="connection">The connection to be opened.</param>
        /// <returns>The same or a replacement IDbConnection.</returns>
        public virtual IDbConnection OnConnectionOpening(IDbConnection connection)
        {
            var args = new DbConnectionEventArgs(connection);
            ConnectionOpening?.Invoke(this, args);
            return args.Connection;
        }

        /// <summary>
        /// Called immediately before a database connection is closed, and invokes the <see cref="IDatabase.ConnectionClosing"/> event.
        /// </summary>
        /// <param name="connection">The connection to be closed.</param>
        public virtual void OnConnectionClosing(IDbConnection connection)
        {
            ConnectionClosing?.Invoke(this, new DbConnectionEventArgs(connection));
        }

        /// <summary>
        /// Called immediately before a database command is executed, and invokes the <see cref="IDatabase.CommandExecuting"/> event.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging of commands, modification of the IDbCommand before it's executed, or any other
        /// custom actions that should be performed before every command
        /// </remarks>
        /// <param name="cmd">The SQL command to be executed.</param>
        public virtual void OnExecutingCommand(IDbCommand cmd)
        {
            CommandExecuting?.Invoke(this, new DbCommandEventArgs(cmd));
        }

        /// <summary>
        /// Called immediately after a database command execution completes, and invokes the <see cref="IDatabase.CommandExecuted"/> event.
        /// </summary>
        /// <remarks>
        /// Override this method to provide custom logging or other actions after every command has completed.
        /// </remarks>
        /// <param name="cmd">The executed SQL command.</param>
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
        /// <inheritdoc/>
        public Task<int> ExecuteAsync(Sql sql)
            => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<int> ExecuteAsync(string sql, params object[] args)
            => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql)
            => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args)
            => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql, args);
#endif

        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <inheritdoc cref="IExecute.Execute(string, object[])"/>
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
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <inheritdoc cref="IExecuteAsync.ExecuteAsync(CancellationToken, string, object[])"/>
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

        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <inheritdoc cref="IExecute.ExecuteScalar(string, object[])"/>
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

                        return (T)Convert.ChangeType(val, u == null ? typeof(T) : u);
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
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <inheritdoc cref="IExecuteAsync.ExecuteScalarAsync{T}(CancellationToken, string, object[])"/>
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

                        return (T)Convert.ChangeType(val, u == null ? typeof(T) : u);
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

        #region Query, QueryAsync : Single-POCO

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

        // TODO: QueryAsync(CancellationToken, string, object[]) takes a caller-provided CancellationToken, but uses a default empty token for wrapped call

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
        public Task QueryAsync<T>(Action<T> action)
            => QueryAsync(action, CancellationToken.None, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, Sql sql)
            => QueryAsync(action, CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, string sql, params object[] args)
            => QueryAsync(action, CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken)
            => QueryAsync(action, cancellationToken, CommandType.Text, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, Sql sql)
            => QueryAsync(action, cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        // TODO: QueryAsync(Action<T>, CancellationToken, string, object[]) takes a caller-provided CancellationToken, but uses a default empty token for wrapped call

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, string sql, params object[] args)
            => QueryAsync(action, CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CommandType commandType)
            => QueryAsync(action, CancellationToken.None, commandType, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CommandType commandType, Sql sql)
            => QueryAsync(action, CancellationToken.None, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CommandType commandType, string sql, params object[] args)
            => QueryAsync(action, CancellationToken.None, commandType, sql, args);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType)
            => QueryAsync(action, cancellationToken, commandType, string.Empty);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType, Sql sql)
            => QueryAsync(action, cancellationToken, commandType, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);
            return ExecuteReaderAsync(action, cancellationToken, commandType, sql, args);
        }
#endif

        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <inheritdoc cref="IQuery.Query{T}(string, object[])"/>
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
        /// <inheritdoc cref="IQueryAsync.QueryAsync{T}(CancellationToken, CommandType, string, object[])"/>
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

        /// <inheritdoc cref="IQueryAsync.QueryAsync{T}(Action{T}, CancellationToken, CommandType, string, object[])"/>
        protected virtual async Task ExecuteReaderAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType, string sql, object[] args)
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
                                action(poco);
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

        #region Query : Multi-POCO

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2) }, null, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql)
            => Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
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
        public IEnumerable<TResult> Query<T1, T2, TResult>(Func<T1, T2, TResult> projector, Sql sql)
            => Query<TResult>(new[] { typeof(T1), typeof(T2) }, projector, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, Sql sql)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3) }, projector, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, Sql sql)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, projector, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, Sql sql)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, projector, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, TResult>(Func<T1, T2, TResult> projector, string sql, params object[] args)
            => Query<TResult>(new[] { typeof(T1), typeof(T2) }, projector, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, string sql, params object[] args)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3) }, projector, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, string sql, params object[] args)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, projector, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, string sql, params object[] args)
            => Query<TResult>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, projector, sql, args);

        /// <inheritdoc/>
        public IEnumerable<TResult> Query<TResult>(Type[] types, object projector, string sql, params object[] args)
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

                    var factory = MultiPocoFactory.GetFactory<TResult>(types, _sharedConnection.ConnectionString, sql, r, _defaultMapper);
                    if (projector == null)
                        projector = MultiPocoFactory.GetAutoMapper(types.ToArray());
                    var bNeedTerminator = false;
                    using (r)
                    {
                        while (true)
                        {
                            TResult poco;
                            try
                            {
                                if (!r.Read())
                                    break;
                                poco = factory(r, projector);
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
                            var poco = (TResult)(projector as Delegate).DynamicInvoke(new object[types.Length]);
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

        /// <inheritdoc/>
        public IGridReader QueryMultiple(Sql sql)
            => QueryMultiple(sql.SQL, sql.Arguments);

        /// <inheritdoc/>
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

        #region Fetch, FetchAsync : Single-POCO

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

        // TODO: FetchAsync(CancellationToken, string, object[]) takes a caller-provided CancellationToken, but uses a default empty token for wrapped call

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

        #region Fetch : Multi-POCO

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
        public List<TResult> Fetch<T1, T2, TResult>(Func<T1, T2, TResult> projector, Sql sql)
            => Query(projector, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, Sql sql)
            => Query(projector, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, Sql sql)
            => Query(projector, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, Sql sql)
            => Query(projector, sql.SQL, sql.Arguments).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, TResult>(Func<T1, T2, TResult> projector, string sql, params object[] args)
            => Query(projector, sql, args).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, string sql, params object[] args)
            => Query(projector, sql, args).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, string sql, params object[] args)
            => Query(projector, sql, args).ToList();

        /// <inheritdoc/>
        public List<TResult> Fetch<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, string sql, params object[] args)
            => Query(projector, sql, args).ToList();

        #endregion

        #region Fetch, FetchAsync : Paged SkipTake

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long maxItemsPerPage)
            => Fetch<T>(page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long maxItemsPerPage, Sql sql)
            => SkipTake<T>((page - 1) * maxItemsPerPage, maxItemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public List<T> Fetch<T>(long page, long maxItemsPerPage, string sql, params object[] args)
            => SkipTake<T>((page - 1) * maxItemsPerPage, maxItemsPerPage, sql, args);

#if ASYNC
        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage)
            => FetchAsync<T>(page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage, Sql sql)
            => FetchAsync<T>(CancellationToken.None, page, maxItemsPerPage, sql);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage, string sql, params object[] args)
            => FetchAsync<T>(CancellationToken.None, page, maxItemsPerPage, sql, args);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage)
            => FetchAsync<T>(cancellationToken, page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql sql)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * maxItemsPerPage, maxItemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string sql, params object[] args)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * maxItemsPerPage, maxItemsPerPage, sql, args);
#endif

        #endregion

        #region Page, PageAsync

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long maxItemsPerPage)
            => Page<T>(page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long maxItemsPerPage, Sql sql)
            => Page<T>(page, maxItemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long maxItemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * maxItemsPerPage, maxItemsPerPage, sql, ref args, out var countSql, out var pageSql);
            return Page<T>(page, maxItemsPerPage, countSql, args, pageSql, args);
        }

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long maxItemsPerPage, Sql countSql, Sql pageSql)
            => Page<T>(page, maxItemsPerPage, countSql.SQL, countSql.Arguments, pageSql.SQL, pageSql.Arguments);

#if ASYNC
        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage)
            => PageAsync<T>(CancellationToken.None, page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, Sql sql)
            => PageAsync<T>(CancellationToken.None, page, maxItemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, string sql, params object[] args)
            => PageAsync<T>(CancellationToken.None, page, maxItemsPerPage, sql, args);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, Sql countSql, Sql pageSql)
            => PageAsync<T>(CancellationToken.None, page, maxItemsPerPage, countSql.SQL, countSql.Arguments, pageSql.SQL, pageSql.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs)
            => PageAsync<T>(CancellationToken.None, page, maxItemsPerPage, countSql, countArgs, pageSql, pageArgs);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage)
            => PageAsync<T>(cancellationToken, page, maxItemsPerPage, string.Empty);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql sql)
            => PageAsync<T>(cancellationToken, page, maxItemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * maxItemsPerPage, maxItemsPerPage, sql, ref args, out var countSql, out var pageSql);
            return PageAsync<T>(cancellationToken, page, maxItemsPerPage, countSql, args, pageSql, args);
        }

        /// <inheritdoc/>
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql countSql, Sql pageSql)
            => PageAsync<T>(cancellationToken, page, maxItemsPerPage, countSql.SQL, countSql.Arguments, pageSql.SQL, pageSql.Arguments);
#endif

        /// <inheritdoc/>
        public Page<T> Page<T>(long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs)
        {
            // Save the one-time command time out and use it for both queries
            var saveTimeout = OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T>
            {
                CurrentPage = page,
                ItemsPerPage = maxItemsPerPage,
                TotalItems = ExecuteScalar<long>(countSql, countArgs)
            };

            result.TotalPages = result.TotalItems / maxItemsPerPage;
            if (result.TotalItems % maxItemsPerPage != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            result.Items = Fetch<T>(pageSql, pageArgs);

            return result;
        }

#if ASYNC
        /// <inheritdoc/>
        public async Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs)
        {
            var saveTimeout = OneTimeCommandTimeout;

            var result = new Page<T>
            {
                CurrentPage = page,
                ItemsPerPage = maxItemsPerPage,
                TotalItems = await ExecuteScalarAsync<long>(cancellationToken, countSql, countArgs).ConfigureAwait(false)
            };

            result.TotalPages = result.TotalItems / maxItemsPerPage;
            if (result.TotalItems % maxItemsPerPage != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            result.Items = await FetchAsync<T>(cancellationToken, pageSql, pageArgs).ConfigureAwait(false);

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
            BuildPageQueries<T>(skip, take, sql, ref args, out var countSql, out var pageSql);
            return Fetch<T>(pageSql, args);
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take)
            => SkipTakeAsync<T>(CancellationToken.None, skip, take, string.Empty);

        // TODO: SkipTakeAsync(long, long, Sql) should forward call to an overload that receives a CancellationToken

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
            BuildPageQueries<T>(skip, take, sql, ref args, out var countSql, out var pageSql);
            return FetchAsync<T>(cancellationToken, pageSql, args);
        }
#endif

        /// <summary>
        /// Starting with a regular <c>SELECT</c> statement, derives the SQL statements required to query a DB for a page of records and the
        /// total number of records.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="skip">The number of records to skip before the start of the page.</param>
        /// <param name="take">The number of records in the page.</param>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters to embed in the SQL statement.</param>
        /// <param name="countSql">When this method returns, contains the SQL statement to query for the total number of records.</param>
        /// <param name="pageSql">When this method returns, contains the SQL statement to retrieve a single page of records.</param>
        /// <exception cref="Exception">Unable to parse the given <paramref name="sql"/> statement.</exception>
        protected virtual void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string countSql, out string pageSql)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            SQLParts parts;
            if (!Provider.PagingUtility.SplitSQL(sql, out parts))
                throw new Exception("Unable to parse SQL statement for paged query");

            pageSql = _provider.BuildPageQuery(skip, take, parts, ref args);
            countSql = parts.SqlCount;
        }

        #endregion

        #region Exists, ExistsAsync

        /// <inheritdoc/>
        /// <example>
        /// <code language="cs" title="Exists">
        /// <![CDATA[
        /// var order = new Order { Id = 7, /*...*/ };
        /// db.Exists<Person>(7); // with a primary key value
        /// db.Exists<Person>(person); // with a poco object (PetaPoco will extract the primary key value)
        /// ]]>
        /// </code>
        /// </example>
        public bool Exists<T>(object pocoOrPrimaryKeyValue)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return Exists<T>($"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                pocoOrPrimaryKeyValue is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(pocoOrPrimaryKeyValue) : pocoOrPrimaryKeyValue);
        }

        /// <inheritdoc/>
        /// <example>
        /// <code language="cs" title="Exists">
        /// <![CDATA[
        /// var peta = "Peta";
        /// db.Exists<Person>("[full_name] = @0", peta);
        /// db.Exists<Person>("WHERE [full_name] = @0", peta); // with long-form syntax ("WHERE" unnecessary, but ok)
        /// ]]>
        /// </code>
        /// </example>
        public bool Exists<T>(string sql, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sql.TrimStart().StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                sql = sql.TrimStart().Substring(5);

            return ExecuteScalar<int>(string.Format(_provider.GetExistsSql(), Provider.EscapeTableName(poco.TableName), sql), args) != 0;
        }

#if ASYNC
        /// <inheritdoc/>
        public Task<bool> ExistsAsync<T>(object pocoOrPrimaryKeyValue)
            => ExistsAsync<T>(CancellationToken.None, pocoOrPrimaryKeyValue);

        /// <inheritdoc/>
        public Task<bool> ExistsAsync<T>(string sql, params object[] args)
            => ExistsAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        public Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKeyValue)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return ExistsAsync<T>(cancellationToken, $"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                pocoOrPrimaryKeyValue is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(pocoOrPrimaryKeyValue) : pocoOrPrimaryKeyValue);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sql.TrimStart().StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
                sql = sql.TrimStart().Substring(5);

            return await ExecuteScalarAsync<int>(cancellationToken, string.Format(_provider.GetExistsSql(),
                Provider.EscapeTableName(poco.TableName), sql), args).ConfigureAwait(false) != 0;
        }
#endif

        #endregion

        // TODO: Some, but not all operation methods use `String.IsNullOrEmpty/IsNullOrWhiteSpace` for string param checks. Most only do null check. Shouldn't all string params check for empty value, in addition to null check?

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
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public object Insert(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/> or <paramref name="poco"/> is null or empty.</exception>
        public object Insert(string tableName, object poco)
        {
            // TODO: Inconsistent use of `ArgumentNullException` vs `ArgumentException` for null/empty string params (find all)
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);

            return ExecuteInsert(tableName, pd == null ? null : pd.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
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
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<object> InsertAsync(object poco)
            => InsertAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/> or <paramref name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(string tableName, object poco)
            => InsertAsync(CancellationToken.None, tableName, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(string tableName, string primaryKeyName, object poco)
            => InsertAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco)
            => InsertAsync(CancellationToken.None, tableName, primaryKeyName, autoIncrement, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<object> InsertAsync(CancellationToken cancellationToken, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsertAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/> or <paramref name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, object poco)
        {
            // TODO: use `string.IsNullOrEmpty(tableName)` (not `tableName == null`)
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsertAsync(cancellationToken, tableName, pd?.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
        {
            // TODO: use `string.IsNullOrEmpty(tableName)` (not `tableName == null`)
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            // TODO: use `string.IsNullOrEmpty(tableName)` (not `tableName == null`)
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

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            // TODO: use `string.IsNullOrEmpty(tableName)` (not `tableName == null`)
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));
            // TODO: use `string.IsNullOrEmpty(tableName)` (not `tableName == null`)
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
                AddParameter(cmd, i.Value.GetValue(poco), i.Value);
            }

            var outputClause = string.Empty;
            if (autoIncrement)
                outputClause = _provider.GetInsertOutputClause(primaryKeyName);

            cmd.CommandText =
                $"INSERT INTO {_provider.EscapeTableName(tableName)} ({string.Join(",", names.ToArray())}){outputClause} VALUES ({string.Join(",", values.ToArray())})";
        }

        /// <inheritdoc cref="IAlterPoco.Insert(string, string, bool, object)"/>
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
        /// <inheritdoc cref="IAlterPocoAsync.InsertAsync(CancellationToken, string, string, bool, object)"/>
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
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public int Update(object poco)
            => Update(poco, null, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public int Update(object poco, IEnumerable<string> columns)
            => Update(poco, null, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public int Update(object poco, object primaryKeyValue)
            => Update(poco, primaryKeyValue, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public int Update(string tableName, string primaryKeyName, object poco)
            => Update(tableName, primaryKeyName, poco, null, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => Update(tableName, primaryKeyName, poco, null, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => Update(tableName, primaryKeyName, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null.</exception>
        public int Update<T>(Sql sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(new Sql($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null or empty.</exception>
        public int Update<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)} {sql}", args);
        }

#if ASYNC
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(object poco)
            => UpdateAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(object poco, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, poco, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(object poco, object primaryKeyValue)
            => UpdateAsync(CancellationToken.None, poco, primaryKeyValue);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(object poco, object primaryKeyValue, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, poco, primaryKeyValue, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, primaryKeyValue);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
            => UpdateAsync(CancellationToken.None, tableName, primaryKeyName, poco, primaryKeyValue, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null.</exception>
        public Task<int> UpdateAsync<T>(Sql sql)
            => UpdateAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null or empty.</exception>
        public Task<int> UpdateAsync<T>(string sql, params object[] args)
            => UpdateAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco)
            => UpdateAsync(cancellationToken, poco, null, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, IEnumerable<string> columns)
            => UpdateAsync(cancellationToken, poco, null, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue)
            => UpdateAsync(cancellationToken, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, null, columns);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue)
            => UpdateAsync(cancellationToken, tableName, primaryKeyName, poco, primaryKeyValue, null);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null.</exception>
        public Task<int> UpdateAsync<T>(CancellationToken cancellationToken, Sql sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return ExecuteAsync(cancellationToken, new Sql($"UPDATE {_provider.EscapeTableName(pd.TableInfo.TableName)}").Append(sql));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="sql"/> is null or empty.</exception>
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
                    AddParameter(cmd, i.Value.GetValue(poco), i.Value);
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
                    AddParameter(cmd, pc.GetValue(poco), pc);
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
            AddParameter(cmd, primaryKeyValue, col);
        }

        /// <inheritdoc cref="IAlterPoco.Update(string, string, object, object, IEnumerable{string})"/>
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
        /// <inheritdoc cref="IAlterPocoAsync.UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
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
        /// <exception cref="InvalidOperationException">Anonymous type does not contain an id for primary key column.</exception>
        public int Delete<T>(object pocoOrPrimaryKeyValue)
        {
            if (pocoOrPrimaryKeyValue.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKeyValue);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKeyValue.GetType().Name.Contains("AnonymousType"))
            {
                var pi = pocoOrPrimaryKeyValue.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException($"Anonymous type does not contain an id for PK column `{pd.TableInfo.PrimaryKey}`.");

                pocoOrPrimaryKeyValue = pi.GetValue(pocoOrPrimaryKeyValue, new object[0]);
            }

            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKeyValue);
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
        /// <exception cref="InvalidOperationException">Anonymous type does not contain an id for primary key column.</exception>
        public Task<int> DeleteAsync<T>(object pocoOrPrimaryKeyValue)
            => DeleteAsync<T>(CancellationToken.None, pocoOrPrimaryKeyValue);

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
        /// <exception cref="InvalidOperationException">Anonymous type does not contain an id for primary key column.</exception>
        public Task<int> DeleteAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKeyValue)
        {
            if (pocoOrPrimaryKeyValue.GetType() == typeof(T))
                return DeleteAsync(cancellationToken, pocoOrPrimaryKeyValue);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKeyValue.GetType().Name.Contains("AnonymousType"))
            {
                var pi = pocoOrPrimaryKeyValue.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException($"Anonymous type does not contain an id for PK column `{pd.TableInfo.PrimaryKey}`.");

                pocoOrPrimaryKeyValue = pi.GetValue(pocoOrPrimaryKeyValue, new object[0]);
            }

            return DeleteAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKeyValue);
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
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public bool IsNew(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return IsNew(pd.TableInfo.PrimaryKey, pd, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="primaryKeyName"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> or <paramref name="primaryKeyName"/> is null.</exception>
        public bool IsNew(string primaryKeyName, object poco)
        {
            if (primaryKeyName == null)
                throw new ArgumentNullException(nameof(primaryKeyName));
            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException(nameof(primaryKeyName));
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return IsNew(primaryKeyName, PocoData.ForObject(poco, primaryKeyName, _defaultMapper), poco);
        }

        /// <inheritdoc cref="IAlterPoco.IsNew(string, object)"/>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="pocoData">The PocoData instance for the specified POCO.</param>
        /// <param name="poco">The POCO instance to check.</param>
        /// <exception cref="InvalidOperationException"><paramref name="primaryKeyName"/> is null or empty, or <paramref name="poco"/> is an
        /// ExpandoObject.</exception>
        /// <exception cref="ArgumentException">The <paramref name="poco"/> doesn't have a property matching the primary key column
        /// name.</exception>
        protected virtual bool IsNew(string primaryKeyName, PocoData pocoData, object poco)
        {
            if (string.IsNullOrEmpty(primaryKeyName) || poco is ExpandoObject)
                throw new InvalidOperationException("IsNew() and Save() are only supported on tables with identity (inc auto-increment) primary key columns");

            object pk;
            PocoColumn pc;
            PropertyInfo pi;
            if (pocoData.Columns.TryGetValue(primaryKeyName, out pc))
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
                return (long)pk == default(long);
            if (type == typeof(int))
                return (int)pk == default(int);
            if (type == typeof(Guid))
                return (Guid)pk == default(Guid);
            if (type == typeof(ulong))
                return (ulong)pk == default(ulong);
            if (type == typeof(uint))
                return (uint)pk == default(uint);
            if (type == typeof(short))
                return (short)pk == default(short);
            if (type == typeof(ushort))
                return (ushort)pk == default(ushort);
            if (type == typeof(decimal))
                return (decimal)pk == default(decimal);

            // Create a default instance and compare
            return pk == Activator.CreateInstance(pk.GetType());
        }

        #endregion

        #region Save, SaveAsync

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public void Save(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            Save(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        // TODO: Inconsistent exception types for same params. Save(string,string,object) throws: first 2 types (2 lines) from IsNew, last 3 types (1 line) from Insert and Update. `primaryKeyName` overlaps with 2 different exception types.

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="primaryKeyName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public void Save(string tableName, string primaryKeyName, object poco)
        {
            if (IsNew(primaryKeyName, poco))
                Insert(tableName, primaryKeyName, true, poco);
            else
                Update(tableName, primaryKeyName, poco);
        }

#if ASYNC
        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task SaveAsync(object poco)
            => SaveAsync(CancellationToken.None, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="primaryKeyName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
        public Task SaveAsync(string tableName, string primaryKeyName, object poco)
            => SaveAsync(CancellationToken.None, tableName, primaryKeyName, poco);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="poco"/> is null.</exception>
        public Task SaveAsync(CancellationToken cancellationToken, object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return SaveAsync(cancellationToken, pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"><paramref name="primaryKeyName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="tableName"/>, <paramref name="primaryKeyName"/>, or <paramref
        /// name="poco"/> is null or empty.</exception>
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
        public Task QueryProcAsync<T>(Action<T> action, string storedProcedureName, params object[] args)
            => QueryProcAsync(action, CancellationToken.None, storedProcedureName, args);

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
        public Task QueryProcAsync<T>(Action<T> action, CancellationToken cancellationToken, string storedProcedureName, params object[] args)
            => ExecuteReaderAsync(action, cancellationToken, CommandType.StoredProcedure, storedProcedureName, args);

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

        /// <summary>
        /// Creates a <see cref="CommandType.Text"/> IDbCommand with the given connection, SQL command text, and arguments.
        /// </summary>
        /// <param name="connection">The connection that will execute the SQL command.</param>
        /// <param name="sql">The SQL command string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An IDbCommand object that represents the SQL command to execute against the provided IDbConnection.</returns>
        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
            => CreateCommand(connection, CommandType.Text, sql, args);

        /// <summary>
        /// Creates an IDbCommand with the given connection, command type, SQL command text, and arguments.
        /// </summary>
        /// <param name="connection">The connection that will execute the SQL command.</param>
        /// <param name="commandType">The type of SQL command to create.</param>
        /// <param name="sql">The SQL command string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An IDbCommand object that represents the SQL command to execute against the provided IDbConnection.</returns>
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
                    AddParameter(cmd, item, null);

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
        /// Creates an IDbParameter with the given name, value, and direction.
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

        /// <summary>
        /// Prepares an IDbDataParameter by setting its properties prior to being added to a command.
        /// </summary>
        /// <param name="param">The IDbDataParameter to which the properties will be set.</param>
        /// <param name="value">The value to be assigned to the IDbDataParameter.</param>
        /// <param name="pocoColumn">The PocoColumn instance for the POCO's column-mapped property.</param>
        private void SetParameterProperties(IDbDataParameter param, object value, PocoColumn pocoColumn)
        {
            // Assign the parameter value
            if (value == null)
            {
                param.Value = DBNull.Value;

                if (pocoColumn?.PropertyInfo.PropertyType.Name == "Byte[]")
                    param.DbType = DbType.Binary;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = _provider.MapParameterValue(value);

                var t = value.GetType();

                if (t == typeof(string) && pocoColumn?.ForceToAnsiString == true)
                {
                    t = typeof(AnsiString);
                    value = value.ToAnsiString();
                }
                if (t == typeof(DateTime) && pocoColumn?.ForceToDateTime2 == true)
                {
                    t = typeof(DateTime2);
                    value = ((DateTime)value).ToDateTime2();
                }

                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    param.Value = Convert.ChangeType(value, ((Enum)value).GetTypeCode());
                }
                else if (t == typeof(Guid) && !_provider.HasNativeGuidSupport)
                {
                    param.Value = value.ToString();
                    param.DbType = DbType.String;
                    param.Size = 40;
                }
                else if (t == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && param.GetType().Name == "SqlCeParameter")
                        param.GetType().GetProperty("SqlDbType").SetValue(param, SqlDbType.NText, null);

                    param.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    param.Value = value;
                }
                else if (t == typeof(AnsiString))
                {
                    var asValue = (value as AnsiString).Value;
                    if (asValue == null)
                    {
                        param.Size = 0;
                        param.Value = DBNull.Value;
                    }
                    else
                    {
                        param.Size = Math.Max(asValue.Length + 1, 4000);
                        param.Value = asValue;
                    }
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    param.DbType = DbType.AnsiString;
                }
                else if (t == typeof(DateTime2))
                {
                    var dt2Value = (value as DateTime2)?.Value;
                    param.Value = dt2Value ?? (object)DBNull.Value;
                    param.DbType = DbType.DateTime2;
                }
                else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    param.GetType().GetProperty("UdtTypeName").SetValue(param, "geography", null); //geography is the equivalent SQL Server Type
                    param.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    param.GetType().GetProperty("UdtTypeName").SetValue(param, "geometry", null); //geography is the equivalent SQL Server Type
                    param.Value = value;
                }
                else if (t == typeof(byte[]))
                {
                    param.Value = value;
                    param.DbType = DbType.Binary;
                }
                else
                {
                    param.Value = value;
                }
            }
        }

        /// <summary>
        /// Adds an IDbDataParameter to a command.
        /// </summary>
        /// <param name="cmd">The SQL command receiving the parameter.</param>
        /// <param name="value">The value to assign to the parameter.</param>
        /// <param name="pc">An optional reference to the PocoColumn instance the value originated from.</param>
        private void AddParameter(IDbCommand cmd, object value, PocoColumn pc)
        {
            // Convert value to from poco type to db type
            if (pc?.PropertyInfo != null)
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
                {
                    var isOracleStoredProc = cmd.CommandType == CommandType.StoredProcedure &&
                        _provider is Providers.OracleDatabaseProvider;

                    // if it's an Oracle stored procedure, only escape the parameter name, don't add param prefix
                    idbParam.ParameterName = isOracleStoredProc ?
                        _provider.EscapeSqlIdentifier(idbParam.ParameterName) :
                        idbParam.ParameterName.EnsureParamPrefix(_paramPrefix);
                }

                cmd.Parameters.Add(idbParam);
            }
            else
            {
                var p = cmd.CreateParameter();

                // only add param prefix if it's not an Oracle stored procedures
                if (!(cmd.CommandType == CommandType.StoredProcedure && _provider is Providers.OracleDatabaseProvider))
                    p.ParameterName = cmd.Parameters.Count.EnsureParamPrefix(_paramPrefix);
                
                SetParameterProperties(p, value, pc);

                cmd.Parameters.Add(p);
            }
        }

        #endregion

        #region Execute Command Helpers

        /// <summary>
        /// Executes an SQL query command and returns a data reader for reading the result set.
        /// </summary>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>A data reader for reading the result set.</returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        internal protected IDataReader ExecuteReaderHelper(IDbCommand cmd)
        {
            return (IDataReader)CommandHelper(cmd, c => c.ExecuteReader());
        }

        /// <summary>
        /// Executes an SQL non-query command and returns the number of rows affected.
        /// </summary>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>The number of rows affected.</returns>
        /// <seealso cref="IDbCommand.ExecuteNonQuery"/>
        internal protected int ExecuteNonQueryHelper(IDbCommand cmd)
        {
            return (int)CommandHelper(cmd, c => c.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes an SQL scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        internal protected object ExecuteScalarHelper(IDbCommand cmd)
        {
            return CommandHelper(cmd, c => c.ExecuteScalar());
        }

        /// <summary>
        /// Executes an SQL command using the provided function and returns the result.
        /// </summary>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <param name="executionFunction">The function to execute the SQL command and return the result.</param>
        /// <returns>The result of the SQL command execution.</returns>
        private object CommandHelper(IDbCommand cmd, Func<IDbCommand, object> executionFunction)
        {
            DoPreExecute(cmd);
            var result = executionFunction(cmd);
            OnExecutedCommand(cmd);
            return result;
        }

#if ASYNC
        /// <summary>
        /// Asynchronously executes an SQL command and returns a data reader for reading the result set.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a data reader for reading the result set.
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        internal protected async Task<IDataReader> ExecuteReaderHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
            {
                var task = CommandHelperAsync(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteReaderAsync(t).ConfigureAwait(false));
                return (IDataReader)await task.ConfigureAwait(false);
            }
            else
                return ExecuteReaderHelper(cmd);
        }

        /// <summary>
        /// Asynchronously executes an SQL non-query command and returns the number of rows affected.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the number of rows affected.
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteNonQuery"/>
        internal protected async Task<int> ExecuteNonQueryHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
            {
                var task = CommandHelperAsync(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteNonQueryAsync(t).ConfigureAwait(false));
                return (int)await task.ConfigureAwait(false);
            }
            else
                return ExecuteNonQueryHelper(cmd);
        }

        /// <summary>
        /// Asynchronously executes an SQL scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the first column of the first row in the result set.
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        internal protected Task<object> ExecuteScalarHelperAsync(CancellationToken cancellationToken, IDbCommand cmd)
        {
            if (cmd is DbCommand dbCommand)
            {
                return CommandHelperAsync(cancellationToken, dbCommand,
                    async (t, c) => await c.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
            }
            else
                return Task.FromResult(ExecuteScalarHelper(cmd));
        }

        // TODO: `CommandHelperAsync(CancellationToken, DbCommand, Func<CancellationToken, DbCommand, Task<object>>` takes DBCommand type; inconsistent with synchronous version `CommandHelper(IDbCommand, Func<IDbCommand, object>)` which uses IDbCommand type (is this intentional?)

        /// <summary>
        /// Asynchronously executes an SQL command using the provided function and returns the result.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="cmd">The SQL command to execute.</param>
        /// <param name="executionFunction">The function to execute the SQL command and return the result.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an object containing the result of the SQL command
        /// execution.
        /// </returns>
        private async Task<object> CommandHelperAsync(CancellationToken cancellationToken, DbCommand cmd, Func<CancellationToken, DbCommand, Task<object>> executionFunction)
        {
            DoPreExecute(cmd);
            var result = await executionFunction(cancellationToken, cmd).ConfigureAwait(false);
            OnExecutedCommand(cmd);
            return result;
        }
#endif

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

        #endregion

        #region Last Command

        /// <inheritdoc/>
        public string LastSQL => _lastSql;

        /// <inheritdoc/>
        public object[] LastArgs => _lastArgs;

        /// <inheritdoc/>
        public string LastCommand => FormatCommand(_lastSql, _lastArgs);

        /// <summary>
        /// Formats an IDbCommand for display.
        /// </summary>
        /// <param name="cmd">The SQL command to format.</param>
        /// <returns>The formatted SQL command.</returns>
        public string FormatCommand(IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        /// <summary>
        /// Formats an SQL statement and its arguments for display.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="args">The parameters embedded in the SQL statement.</param>
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
                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i]?.GetType().Name ?? "Unknown Type", args[i]);
                }

                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        #endregion

        #region Configuration Properties

        /// <summary>
        /// <inheritdoc/>
        /// Default is <see cref="ConventionMapper"/>.
        /// </summary>
        public IMapper DefaultMapper => _defaultMapper;

        /// <inheritdoc/>
        public string ConnectionString => _connectionString;

        /// <inheritdoc/>
        public IProvider Provider => _provider;

        /// <summary>
        /// <inheritdoc/>
        /// Default is <see langword="null"/>.
        /// </summary>
        /// <value>If <see langword="null"/>, the default isolation level of the underlying <see cref="Provider"/> is used.</value>
        /// <exception cref="InvalidOperationException">If changed while a transaction is in progress.</exception>
        public IsolationLevel? IsolationLevel
        {
            get => _isolationLevel;
            set => _isolationLevel = _transaction == null
                    ? value
                    : throw new InvalidOperationException("Isolation level can not be changed during a transaction.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// Default is <see langword="false"/>.
        /// </summary>
        /// <inheritdoc/>
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// Default is <see langword="true"/>.
        /// </summary>
        /// <value>If <see langword="true"/>, PetaPoco will automatically generate the <c>SELECT</c> portion of the query when needed if not
        /// explicitly provided in the supplied SQL statement.</value>
        /// <example>
        /// <para/>In the following example, all three queries below will result in the same outcome:
        /// <code language="cs" title="EnableAutoSelect">
        /// <![CDATA[
        /// var note = db.Single<Note>("WHERE `id` = @0", 123);
        /// var note = db.Single<Note>("FROM `notes` WHERE `id` = @0", 123);
        /// var note = db.Single<Note>("SELECT * FROM `notes` WHERE `id` = @0", 123);
        /// ]]>
        /// </code>
        /// The generated SQL produced by PetaPoco, shown below, is identical for all three:
        /// <code language="sql">
        /// <![CDATA[
        /// SELECT [Note].[Id], [Note].[CreatedOn], [Note].[Text] FROM [Note] WHERE [Id] = @0;
        /// ]]>
        /// </code>
        /// </example>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        /// <inheritdoc/> Default is <see langword="true"/>.
        /// </summary>
        /// <value>If <see langword="true"/>, parameters can be named "?myparam" in the SQL string, and populated from properties of the
        /// passed-in argument values.</value>
        public bool EnableNamedParams { get; set; }

        /// <summary>
        /// <inheritdoc/> Default is 0.
        /// </summary>
        /// <value>If 0, PetaPoco will use the default <see cref="IDbCommand.CommandTimeout"/> value for the active database <see
        /// cref="Provider"/> (typically 30 seconds).</value>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// <inheritdoc/>
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
        /// <inheritdoc cref="Database(string, IProvider, IMapper)"/>
        public Database(string connectionString, IMapper defaultMapper = null)
            : base(connectionString, typeof(TDatabaseProvider).Name, defaultMapper)
        {
        }
    }
}
