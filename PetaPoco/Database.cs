using System;
using System.Collections.Generic;
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

#if !NETSTANDARD
using System.Configuration;
#endif

namespace PetaPoco
{
    /// <inheritdoc />
    public class Database : IDatabase
    {
#region Member Fields

        private IMapper _defaultMapper;
        private string _connectionString;
        private IProvider _provider;
        private IDbConnection _sharedConnection;
        private IDbTransaction _transaction;
        private int _sharedConnectionDepth;
        private int _transactionDepth;
        private bool _transactionCancelled;
        private string _lastSql;
        private object[] _lastArgs;
        private string _paramPrefix;
        private DbProviderFactory _factory;
        private IsolationLevel? _isolationLevel;

#endregion

#region Constructors

#if !NETSTANDARD
        /// <summary>
        ///     Constructs an instance using the first connection string found in the app/web configuration file.
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
        ///     Constructs an instance using a supplied connection string name. The actual connection string and provider will be
        ///     read from app/web.config.
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
        ///     Constructs an instance using a supplied IDbConnection.
        /// </summary>
        /// <param name="connection">The IDbConnection to use.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <remarks>
        ///     The supplied IDbConnection will not be closed/disposed by PetaPoco - that remains
        ///     the responsibility of the caller.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connection" /> is null or empty.</exception>
        public Database(DbConnection connection, IMapper defaultMapper = null)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            SetupFromConnection(connection);
            Initialise(DatabaseProvider.Resolve(_sharedConnection.GetType(), false, _connectionString), defaultMapper);
        }

        private void SetupFromConnection(DbConnection connection)
        {
            _sharedConnection = connection;
            _connectionString = connection.ConnectionString;

            // Prevent closing external connection
            _sharedConnectionDepth = 2;
        }

        /// <summary>
        ///     Constructs an instance using a supplied connection string and provider name.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="providerName">The database provider name.</param>
        /// <param name="defaultMapper">The default mapper to use when no specific mapper has been registered.</param>
        /// <remarks>
        ///     PetaPoco will automatically close and dispose any connections it creates.
        /// </remarks>
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
        ///     Constructs an instance using the supplied connection string and DbProviderFactory.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="factory">The DbProviderFactory to use for instantiating IDbConnection's.</param>
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
        ///     Constructs an instance using the supplied provider and optional default mapper.
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
        ///     Constructs an instance using the supplied <paramref name="configuration" />.
        /// </summary>
        /// <param name="configuration">The configuration for constructing an instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when no configuration string is configured and app/web config does
        ///     any connection string registered.
        /// </exception>
        /// <exception cref="InvalidOperationException">Thrown when a connection string configured and no provider is configured.</exception>
        public Database(IDatabaseBuildConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var settings = (IBuildConfigurationSettings) configuration;

            IMapper defaultMapper = null;
            settings.TryGetSetting<IMapper>(DatabaseConfigurationExtensions.DefaultMapper, v => defaultMapper = v);

            IProvider provider = null;
            DbConnection connection = null;
            string providerName = null;
#if !NETSTANDARD
            ConnectionStringSettings entry = null;
#endif

            settings.TryGetSetting<IProvider>(DatabaseConfigurationExtensions.Provider, p => provider = p);
            settings.TryGetSetting<DbConnection>(DatabaseConfigurationExtensions.Connection, c => connection = c);
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

            settings.TryGetSetting<EventHandler<DbConnectionEventArgs>>(DatabaseConfigurationExtensions.ConnectionOpened, v => ConnectionOpened += v);
            settings.TryGetSetting<EventHandler<DbConnectionEventArgs>>(DatabaseConfigurationExtensions.ConnectionClosing, v => ConnectionClosing += v);
            settings.TryGetSetting<EventHandler<DbTransactionEventArgs>>(DatabaseConfigurationExtensions.TransactionStarted, v => TransactionStarted += v);
            settings.TryGetSetting<EventHandler<DbTransactionEventArgs>>(DatabaseConfigurationExtensions.TransactionEnding, v => TransactionEnding += v);
            settings.TryGetSetting<EventHandler<DbCommandEventArgs>>(DatabaseConfigurationExtensions.CommandExecuting, v => CommandExecuting += v);
            settings.TryGetSetting<EventHandler<DbCommandEventArgs>>(DatabaseConfigurationExtensions.CommandExecuted, v => CommandExecuted += v);
            settings.TryGetSetting<EventHandler<ExceptionEventArgs>>(DatabaseConfigurationExtensions.ExceptionThrown, v => ExceptionThrown += v);
        }

        /// <summary>
        ///     Provides common initialization for the various constructors.
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

#region Internal operations

        internal void DoPreExecute(IDbCommand cmd)
        {
            if (CommandTimeout > 0 || OneTimeCommandTimeout > 0)
            {
                cmd.CommandTimeout = OneTimeCommandTimeout > 0 ? OneTimeCommandTimeout : CommandTimeout;
                OneTimeCommandTimeout = 0;
            }

            OnExecutingCommand(cmd);

            _lastSql = cmd.CommandText;
            _lastArgs = cmd.Parameters.Cast<IDataParameter>().Select(parameter => parameter.Value).ToArray();
        }

#endregion

#region Connection Management

        /// <summary>
        ///     When set to true the first opened connection is kept alive until <see cref="CloseSharedConnection" />
        ///     or <see cref="Dispose" /> is called.
        /// </summary>
        /// <seealso cref="OpenSharedConnection" />
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        ///     Provides access to the currently open shared connection.
        /// </summary>
        /// <returns>
        ///     The currently open connection, or <c>Null</c>.
        /// </returns>
        /// <seealso cref="OpenSharedConnection" />
        /// <seealso cref="CloseSharedConnection" />
        /// <seealso cref="KeepConnectionAlive" />
        public IDbConnection Connection => _sharedConnection;

        /// <summary>
        ///     Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        ///     Calls to <see cref="OpenSharedConnection" />/<see cref="CloseSharedConnection" /> are reference
        ///     counted and should be balanced
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
        ///     The async version of <see cref="OpenSharedConnection" />.
        /// </summary>
        public Task OpenSharedConnectionAsync() => OpenSharedConnectionAsync(CancellationToken.None);

        /// <summary>
        ///     The async version of <see cref="OpenSharedConnection" />.
        /// </summary>
        public async Task OpenSharedConnectionAsync(CancellationToken cancellationToken)
        {
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _connectionString;

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
        ///     Releases the shared connection.
        /// </summary>
        /// <remarks>
        ///     Calls to <see cref="OpenSharedConnection" />/<see cref="CloseSharedConnection" /> are reference
        ///     counted and should be balanced
        /// </remarks>
        /// <seealso cref="Connection" />
        /// <seealso cref="OpenSharedConnection" />
        /// <seealso cref="KeepConnectionAlive" />
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
        ///     Alias for <see cref="CloseSharedConnection" />.
        /// </summary>
        /// <remarks>
        ///     Only useful when making use of the .net `using` language feature.
        /// </remarks>
        public void Dispose()
        {
            // Automatically close one open connection reference
            //  (Works with KeepConnectionAlive and manually opening a shared connection)
            CloseSharedConnection();
        }

#endregion

#region Transaction Management

        /// <inheritdoc />
        IDbTransaction ITransactionAccessor.Transaction => _transaction;

        /// <inheritdoc />
        public ITransaction GetTransaction() => new Transaction(this);

        /// <summary>
        ///     Called when a transaction starts.
        /// </summary>
        public virtual void OnBeginTransaction()
        {
            TransactionStarted?.Invoke(this, new DbTransactionEventArgs(_transaction));
        }

        /// <summary>
        ///     Called when a transaction ends.
        /// </summary>
        public virtual void OnEndTransaction()
        {
            TransactionEnding?.Invoke(this, new DbTransactionEventArgs(_transaction));
        }

        /// <inheritdoc />
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
        /// <inheritdoc />
        public Task BeginTransactionAsync() => BeginTransactionAsync(CancellationToken.None);

        /// <inheritdoc />
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transactionDepth++;

            if (_transactionDepth == 1)
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                _transaction = !_isolationLevel.HasValue ? _sharedConnection.BeginTransaction() : _sharedConnection.BeginTransaction(_isolationLevel.Value);
                _transactionCancelled = false;
                OnBeginTransaction();
            }
        }
#endif

        /// <summary>
        ///     Internal helper to cleanup transaction
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

        /// <inheritdoc />
        public void AbortTransaction()
        {
            _transactionCancelled = true;
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        /// <inheritdoc />
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

#endregion

#region Command Management

        /// <summary>
        ///     Add a parameter to a DB command
        /// </summary>
        /// <param name="cmd">A reference to the IDbCommand to which the parameter is to be added</param>
        /// <param name="value">The value to assign to the parameter</param>
        /// <param name="pi">Optional, a reference to the property info of the POCO property from which the value is coming.</param>
        private void AddParam(IDbCommand cmd, object value, PropertyInfo pi)
        {
            // Convert value to from poco type to db type
            if (pi != null)
            {
                var mapper = Mappers.GetMapper(pi.DeclaringType, _defaultMapper);
                var fn = mapper.GetToDbConverter(pi);
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
                SetParameterProperties(p, value, pi);

                cmd.Parameters.Add(p);
            }
        }

        private void SetParameterProperties(IDbDataParameter p, object value, PropertyInfo pi)
        {
            // Assign the parameter value
            if (value == null)
            {
                p.Value = DBNull.Value;

                if (pi?.PropertyType.Name == "Byte[]")
                    p.DbType = DbType.Binary;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = _provider.MapParameterValue(value);

                var t = value.GetType();
                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = Convert.ChangeType(value, ((Enum) value).GetTypeCode());
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
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max((value as AnsiString).Value.Length + 1, 4000);
                    p.Value = (value as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
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
                else
                {
                    p.Value = value;
                }
            }
        }

        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args) => CreateCommand(connection, CommandType.Text, sql, args);

        public IDbCommand CreateCommand(IDbConnection connection, CommandType commandType, string sql, params object[] args)
        {
            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
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

            _provider.PreExecute(cmd);

            if (!string.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }

#endregion

#region Exception Reporting and Logging

        /// <summary>
        ///     Called if an exception occurs during processing of a DB operation.  Override to provide custom logging/handling.
        /// </summary>
        /// <param name="x">The exception instance</param>
        /// <returns>True to re-throw the exception, false to suppress it</returns>
        public virtual bool OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());
            System.Diagnostics.Debug.WriteLine(LastCommand);

            var args = new ExceptionEventArgs(x);
            ExceptionThrown?.Invoke(this, new ExceptionEventArgs(x));
            return args.Raise;
        }

        /// <summary>
        ///     Called when DB connection opened
        /// </summary>
        /// <param name="conn">The newly opened IDbConnection</param>
        /// <returns>The same or a replacement IDbConnection</returns>
        /// <remarks>
        ///     Override this method to provide custom logging of opening connection, or
        ///     to provide a proxy IDbConnection.
        /// </remarks>
        public virtual IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            var args = new DbConnectionEventArgs(conn);
            ConnectionOpened?.Invoke(this, args);
            return args.Connection;
        }

        /// <summary>
        ///     Called when DB connection closed
        /// </summary>
        /// <param name="conn">The soon to be closed IDBConnection</param>
        public virtual void OnConnectionClosing(IDbConnection conn)
        {
            ConnectionClosing?.Invoke(this, new DbConnectionEventArgs(conn));
        }

        /// <summary>
        ///     Called just before an DB command is executed
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        /// <remarks>
        ///     Override this method to provide custom logging of commands and/or
        ///     modification of the IDbCommand before it's executed
        /// </remarks>
        public virtual void OnExecutingCommand(IDbCommand cmd)
        {
            CommandExecuting?.Invoke(this, new DbCommandEventArgs(cmd));
        }

        /// <summary>
        ///     Called on completion of command execution
        /// </summary>
        /// <param name="cmd">The IDbCommand that finished executing</param>
        public void OnExecutedCommand(IDbCommand cmd)
        {
            CommandExecuted?.Invoke(this, new DbCommandEventArgs(cmd));
        }

#endregion

#region operation: Execute 

        /// <inheritdoc />
        public int Execute(string sql, params object[] args) => ExecuteInternal(CommandType.Text, sql, args);

        /// <inheritdoc />
        public int Execute(Sql sql) => Execute(sql.SQL, sql.Arguments);

        protected virtual int ExecuteInternal(CommandType commandType, string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var rowsAffected = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return rowsAffected;
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

        public Task<int> ExecuteAsync(string sql, params object[] args) => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql, args);

        public Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args)
            => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql, args);

        public Task<int> ExecuteAsync(Sql sql) => ExecuteInternalAsync(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        public Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql) => ExecuteInternalAsync(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        protected virtual async Task<int> ExecuteInternalAsync(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var rowsAffected = cmd is DbCommand dbCommandAsync
                            ? await dbCommandAsync.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false)
                            : cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return rowsAffected;
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

#region operation: ExecuteScalar

        /// <inheritdoc />
        public T ExecuteScalar<T>(string sql, params object[] args) => ExecuteScalarInternal<T>(CommandType.Text, sql, args);

        /// <inheritdoc />
        public T ExecuteScalar<T>(Sql sql) => ExecuteScalar<T>(sql.SQL, sql.Arguments);

        protected virtual T ExecuteScalarInternal<T>(CommandType commandType, string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);

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

        /// <inheritdoc />
        public Task<T> ExecuteScalarAsync<T>(string sql, params object[] args) => ExecuteScalarInternalAsync<T>(CancellationToken.None, CommandType.Text, sql, args);

        /// <inheritdoc />
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => ExecuteScalarInternalAsync<T>(cancellationToken, CommandType.Text, sql, args);

        /// <inheritdoc />
        public Task<T> ExecuteScalarAsync<T>(Sql sql) => ExecuteScalarInternalAsync<T>(CancellationToken.None, CommandType.Text, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, Sql sql)
            => ExecuteScalarInternalAsync<T>(cancellationToken, CommandType.Text, sql.SQL, sql.Arguments);

        protected virtual async Task<T> ExecuteScalarInternalAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args)
        {
            try
            {
                await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                    {
                        var val = cmd is DbCommand cmdAsync ? await cmdAsync.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) : cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);

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

#region operation: Fetch

        /// <inheritdoc />
        public List<T> Fetch<T>() => Fetch<T>(string.Empty);

        /// <inheritdoc />
        public List<T> Fetch<T>(string sql, params object[] args) => Query<T>(sql, args).ToList();

        /// <inheritdoc />
        public List<T> Fetch<T>(Sql sql) => Fetch<T>(sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public List<T> Fetch<T>(long page, long itemsPerPage) => Fetch<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args) => SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);

        /// <inheritdoc />
        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql) => SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);

#if ASYNC
        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>() => FetchAsync<T>(CommandType.Text, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CommandType commandType) => FetchAsync<T>(CommandType.Text, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken) => FetchAsync<T>(CommandType.Text, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken) => FetchAsync<T>(commandType, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(string sql, params object[] args) => FetchAsync<T>(CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CommandType commandType, string sql, params object[] args) => FetchAsync<T>(commandType, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => FetchAsync<T>(CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public async Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args)
        {
            var pocos = new List<T>();
            await QueryAsync<T>(p => pocos.Add(p), commandType, cancellationToken, sql, args).ConfigureAwait(false);
            return pocos;
        }

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(Sql sql) => FetchAsync<T>(CommandType.Text, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CommandType commandType, Sql sql) => FetchAsync<T>(commandType, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, Sql sql) => FetchAsync<T>(CommandType.Text, cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, Sql sql)
            => FetchAsync<T>(commandType, cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage) => FetchAsync<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage) => FetchAsync<T>(cancellationToken, page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args) => FetchAsync<T>(CancellationToken.None, page, itemsPerPage, sql, args);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * itemsPerPage, itemsPerPage, sql, args);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql) => FetchAsync<T>(CancellationToken.None, page, itemsPerPage, sql);

        /// <inheritdoc />
        public Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql)
            => SkipTakeAsync<T>(cancellationToken, (page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);

#endif

#endregion

#region operation: Page

        /// <summary>
        ///     Starting with a regular SELECT statement, derives the SQL statements required to query a
        ///     DB for a page of records and the total number of records
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="skip">The number of rows to skip before the start of the page</param>
        /// <param name="take">The number of rows in the page</param>
        /// <param name="sql">The original SQL select statement</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <param name="sqlCount">Outputs the SQL statement to query for the total number of matching rows</param>
        /// <param name="sqlPage">Outputs the SQL statement to retrieve a single page of matching rows</param>
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Page<T> Page<T>(long page, long itemsPerPage) => Page<T>(page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out var sqlCount, out var sqlPage);
            return Page<T>(page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        /// <inheritdoc />
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql) => Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => Page<T>(page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);

#if ASYNC

        /// <inheritdoc />
        public async Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage,
                                                object[] pageArgs)
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

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sqlCount, countArgs, sqlPage, pageArgs);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage) => PageAsync<T>(cancellationToken, page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage) => PageAsync<T>(CancellationToken.None, page, itemsPerPage, string.Empty);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args)
        {
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out var sqlCount, out var sqlPage);
            return PageAsync<T>(cancellationToken, page, itemsPerPage, sqlCount, args, sqlPage, args);
        }

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, params object[] args) => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sql, args);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql)
            => PageAsync<T>(cancellationToken, page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sql) => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => PageAsync<T>(cancellationToken, page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);

        /// <inheritdoc />
        public Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
            => PageAsync<T>(CancellationToken.None, page, itemsPerPage, sqlCount.SQL, sqlCount.Arguments, sqlPage.SQL, sqlPage.Arguments);

#endif

#endregion

#region operation: SkipTake

        /// <inheritdoc />
        public List<T> SkipTake<T>(long skip, long take) => SkipTake<T>(skip, take, string.Empty);

        /// <inheritdoc />
        public List<T> SkipTake<T>(long skip, long take, Sql sql) => SkipTake<T>(skip, take, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            BuildPageQueries<T>(skip, take, sql, ref args, out var sqlCount, out var sqlPage);
            return Fetch<T>(sqlPage, args);
        }

#if ASYNC

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take) => SkipTakeAsync<T>(cancellationToken, skip, take, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take) => SkipTakeAsync<T>(CancellationToken.None, skip, take, string.Empty);

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, string sql, params object[] args)
        {
            BuildPageQueries<T>(skip, take, sql, ref args, out var sqlCount, out var sqlPage);
            return FetchAsync<T>(cancellationToken, sqlPage, args);
        }

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args) => SkipTakeAsync<T>(CancellationToken.None, skip, take, sql, args);

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, Sql sql)
            => SkipTakeAsync<T>(cancellationToken, skip, take, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql) => SkipTakeAsync<T>(skip, take, sql.SQL, sql.Arguments);

#endif

#endregion

#region operation: Query

        /// <inheritdoc />
        public IEnumerable<T> Query<T>() => Query<T>(string.Empty);

        /// <inheritdoc />
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return ExecuteReader<T>(CommandType.Text, sql, args);
        }

        /// <inheritdoc />
        public IEnumerable<T> Query<T>(Sql sql) => Query<T>(sql.SQL, sql.Arguments);

#if ASYNC
        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback) => QueryAsync(receivePocoCallback, CommandType.Text, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType) => QueryAsync(receivePocoCallback, commandType, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken)
            => QueryAsync(receivePocoCallback, CommandType.Text, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken)
            => QueryAsync(receivePocoCallback, commandType, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, commandType, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string sql, params object[] args)
            => QueryAsync(receivePocoCallback, CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return ExecuteReaderAsync(receivePocoCallback, commandType, cancellationToken, sql, args);
        }

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, Sql sql) => QueryAsync(receivePocoCallback, CommandType.Text, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, Sql sql)
            => QueryAsync(receivePocoCallback, commandType, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, Sql sql)
            => QueryAsync(receivePocoCallback, CommandType.Text, cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken, Sql sql)
            => QueryAsync(receivePocoCallback, commandType, cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>() => QueryAsync<T>(CommandType.Text, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType) => QueryAsync<T>(commandType, CancellationToken.None, string.Empty);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken) => QueryAsync<T>(CommandType.Text, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, CancellationToken cancellationToken) => QueryAsync<T>(commandType, cancellationToken, string.Empty);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(string sql, params object[] args) => QueryAsync<T>(CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, string sql, params object[] args) => QueryAsync<T>(commandType, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => QueryAsync<T>(CommandType.Text, CancellationToken.None, sql, args);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AutoSelectHelper.AddSelectClause<T>(_provider, sql, _defaultMapper);

            return ExecuteReaderAsync<T>(commandType, cancellationToken, sql, args);
        }

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(Sql sql) => QueryAsync<T>(CommandType.Text, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, Sql sql) => QueryAsync<T>(commandType, CancellationToken.None, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, Sql sql) => QueryAsync<T>(CommandType.Text, cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, CancellationToken cancellationToken, Sql sql)
            => QueryAsync<T>(commandType, cancellationToken, sql.SQL, sql.Arguments);

        protected virtual async Task ExecuteReaderAsync<T>(Action<T> processPoco, CommandType commandType, CancellationToken cancellationToken, string sql, object[] args)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, commandType, sql, args))
                {
                    IDataReader reader;
                    var pd = PocoData.ForType(typeof(T), _defaultMapper);
                    var cmdAsync = cmd as DbCommand;

                    try
                    {
                        if (cmdAsync != null)
                            reader = await cmdAsync.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        else
                            reader = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    }
                    catch (Exception e)
                    {
                        if (OnException(e))
                            throw;
                        return;
                    }

                    var readerAsync = reader as DbDataReader;
                    var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, reader.FieldCount, reader, _defaultMapper) as Func<IDataReader, T>;

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

        protected virtual async Task<IAsyncReader<T>> ExecuteReaderAsync<T>(CommandType commandType, CancellationToken cancellationToken, string sql, object[] args)
        {
            await OpenSharedConnectionAsync(cancellationToken).ConfigureAwait(false);
            var cmd = CreateCommand(_sharedConnection, commandType, sql, args);
            IDataReader reader = null;
            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            try
            {
                if (cmd is DbCommand cmdAsync)
                    reader = await cmdAsync.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                else
                    reader = cmd.ExecuteReader();

                OnExecutedCommand(cmd);
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

            var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, reader.FieldCount, reader, _defaultMapper) as Func<IDataReader, T>;

            return new AsyncReader<T>(this, cmd, reader, factory);
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
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        if (OnException(x))
                            throw;
                        yield break;
                    }

                    var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, 0, r.FieldCount, r, _defaultMapper) as Func<IDataReader, T>;
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

#endregion

#region operation: Exists

        /// <inheritdoc />
        public bool Exists<T>(string sqlCondition, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sqlCondition.TrimStart().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart().Substring(5);

            return ExecuteScalar<int>(string.Format(_provider.GetExistsSql(), Provider.EscapeTableName(poco.TableName), sqlCondition), args) != 0;
        }

        /// <inheritdoc />
        public bool Exists<T>(object primaryKey)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return Exists<T>($"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                primaryKey is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(primaryKey) : primaryKey);
        }

#if ASYNC
        public Task<bool> ExistsAsync<T>(object primaryKey) => ExistsAsync<T>(CancellationToken.None, primaryKey);

        public Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, object primaryKey)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper);
            return ExistsAsync<T>(cancellationToken, $"{_provider.EscapeSqlIdentifier(poco.TableInfo.PrimaryKey)}=@0",
                primaryKey is T ? poco.Columns[poco.TableInfo.PrimaryKey].GetValue(primaryKey) : primaryKey);
        }

        public Task<bool> ExistsAsync<T>(string sqlCondition, params object[] args) => ExistsAsync<T>(CancellationToken.None, sqlCondition, args);

        public async Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, string sqlCondition, params object[] args)
        {
            var poco = PocoData.ForType(typeof(T), _defaultMapper).TableInfo;

            if (sqlCondition.TrimStart().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                sqlCondition = sqlCondition.TrimStart().Substring(5);

            return await ExecuteScalarAsync<int>(cancellationToken, string.Format(_provider.GetExistsSql(), Provider.EscapeTableName(poco.TableName), sqlCondition), args).ConfigureAwait(false) != 0;
        }
#endif

#endregion

#region operation: linq style (Exists, Single, SingleOrDefault etc...)

        /// <inheritdoc />
        public T Single<T>(object primaryKey) => Single<T>(GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc />
        public T SingleOrDefault<T>(object primaryKey) => SingleOrDefault<T>(GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc />
        public T Single<T>(string sql, params object[] args) => Query<T>(sql, args).Single();

        /// <inheritdoc />
        public T SingleOrDefault<T>(string sql, params object[] args) => Query<T>(sql, args).SingleOrDefault();

        /// <inheritdoc />
        public T First<T>(string sql, params object[] args) => Query<T>(sql, args).First();

        /// <inheritdoc />
        public T FirstOrDefault<T>(string sql, params object[] args) => Query<T>(sql, args).FirstOrDefault();

        /// <inheritdoc />
        public T Single<T>(Sql sql) => Query<T>(sql).Single();

        /// <inheritdoc />
        public T SingleOrDefault<T>(Sql sql) => Query<T>(sql).SingleOrDefault();

        /// <inheritdoc />
        public T First<T>(Sql sql) => Query<T>(sql).First();

        /// <inheritdoc />
        public T FirstOrDefault<T>(Sql sql) => Query<T>(sql).FirstOrDefault();

#if ASYNC

        /// <inheritdoc />
        public Task<T> SingleAsync<T>(object primaryKey) => SingleAsync<T>(CancellationToken.None, primaryKey);

        /// <inheritdoc />
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken, object primaryKey) => SingleAsync<T>(cancellationToken, GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc />
        public Task<T> SingleAsync<T>(string sql, params object[] args) => SingleAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc />
        public async Task<T> SingleAsync<T>(CancellationToken cancellationToken, string sql, params object[] args) => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).Single();

        /// <inheritdoc />
        public Task<T> SingleAsync<T>(Sql sql) => SingleAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc />
        public Task<T> SingleAsync<T>(CancellationToken cancellationToken, Sql sql) => SingleAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync<T>(Sql sql) => SingleOrDefaultAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql) => SingleOrDefaultAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync<T>(object primaryKey) => SingleOrDefaultAsync<T>(CancellationToken.None, primaryKey);

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, object primaryKey)
            => SingleOrDefaultAsync<T>(cancellationToken, GenerateSingleByKeySql<T>(primaryKey));

        /// <inheritdoc />
        public Task<T> SingleOrDefaultAsync<T>(string sql, params object[] args) => SingleOrDefaultAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc />
        public async Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).SingleOrDefault();

        /// <inheritdoc />
        public Task<T> FirstAsync<T>(string sql, params object[] args) => FirstAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc />
        public async Task<T> FirstAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).First();

        /// <inheritdoc />
        public Task<T> FirstAsync<T>(Sql sql) => FirstAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc />
        public Task<T> FirstAsync<T>(CancellationToken cancellationToken, Sql sql) => FirstAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

        /// <inheritdoc />
        public Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args) => FirstOrDefaultAsync<T>(CancellationToken.None, sql, args);

        /// <inheritdoc />
        public async Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args)
            => (await FetchAsync<T>(cancellationToken, sql, args).ConfigureAwait(false)).FirstOrDefault();

        /// <inheritdoc />
        public Task<T> FirstOrDefaultAsync<T>(Sql sql) => FirstOrDefaultAsync<T>(CancellationToken.None, sql);

        /// <inheritdoc />
        public Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql) => FirstOrDefaultAsync<T>(cancellationToken, sql.SQL, sql.Arguments);

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

#region operation: Insert

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        public object Insert(string tableName, object poco)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);

            return ExecuteInsert(tableName, pd == null ? null : pd.TableInfo.PrimaryKey, pd != null && pd.TableInfo.AutoIncrement, poco);
        }

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
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
            var autoIncrement = pd == null || pd.TableInfo.AutoIncrement ||
                                t.Name.Contains("AnonymousType") && !t.GetProperties().Any(p => p.Name.Equals(primaryKeyName, StringComparison.OrdinalIgnoreCase));

            return ExecuteInsert(tableName, primaryKeyName, autoIncrement, poco);
        }

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="tableName">The name of the table to insert into</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     Inserts a poco into a table.  If the poco has a property with the same name
        ///     as the primary key the id of the new record is assigned to it.  Either way,
        ///     the new id is returned.
        /// </remarks>
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

        /// <summary>
        ///     Performs an SQL Insert
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        public object Insert(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteInsert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        private object ExecuteInsert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        var names = new List<string>();
                        var values = new List<string>();
                        var index = 0;
                        foreach (var i in pd.Columns)
                        {
                            // Don't insert result columns
                            if (i.Value.ResultColumn)
                                continue;

                            // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                            if (autoIncrement && primaryKeyName != null && string.Compare(i.Key, primaryKeyName, true) == 0)
                            {
                                // Setup auto increment expression
                                string autoIncExpression = _provider.GetAutoIncrementExpression(pd.TableInfo);
                                if (autoIncExpression != null)
                                {
                                    names.Add(i.Key);
                                    values.Add(autoIncExpression);
                                }

                                continue;
                            }

                            names.Add(_provider.EscapeSqlIdentifier(i.Key));
                            values.Add(string.Format(i.Value.InsertTemplate ?? "{0}{1}", _paramPrefix, index++));
                            AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
                        }

                        string outputClause = string.Empty;
                        if (autoIncrement)
                        {
                            outputClause = _provider.GetInsertOutputClause(primaryKeyName);
                        }

                        cmd.CommandText = string.Format("INSERT INTO {0} ({1}){2} VALUES ({3})", _provider.EscapeTableName(tableName), string.Join(",", names.ToArray()),
                            outputClause, string.Join(",", values.ToArray()));

                        if (!autoIncrement)
                        {
                            DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            OnExecutedCommand(cmd);

                            PocoColumn pkColumn;
                            if (primaryKeyName != null && pd.Columns.TryGetValue(primaryKeyName, out pkColumn))
                                return pkColumn.GetValue(poco);
                            else
                                return null;
                        }

                        object id = _provider.ExecuteInsert(this, cmd, primaryKeyName);

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null && !poco.GetType().Name.Contains("AnonymousType"))
                        {
                            PocoColumn pc;
                            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                            {
                                pc.SetValue(poco, pc.ChangeType(id));
                            }
                        }

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

#endregion

#region operation: Update

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected records</returns>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return ExecuteUpdate(tableName, primaryKeyName, poco, primaryKeyValue, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return ExecuteUpdate(tableName, primaryKeyName, poco, primaryKeyValue, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return Update(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException(nameof(tableName));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentNullException(nameof(primaryKeyName));

            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            return ExecuteUpdate(tableName, primaryKeyName, poco, null, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco, IEnumerable<string> columns)
        {
            return Update(poco, null, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco)
        {
            return Update(poco, null, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco, object primaryKeyValue)
        {
            return Update(poco, primaryKeyValue, null);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        public int Update(object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return ExecuteUpdate(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Update<T>(string sql, params object[] args)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(string.Format("UPDATE {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL update and condition clause (ie: everything after "UPDATE
        ///     tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        public int Update<T>(Sql sql)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(new Sql(string.Format("UPDATE {0}", _provider.EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }

        private int ExecuteUpdate(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var sb = new StringBuilder();
                        var index = 0;
                        var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                        if (columns == null)
                        {
                            foreach (var i in pd.Columns)
                            {
                                // Don't update the primary key, but grab the value if we don't have it
                                if (string.Compare(i.Key, primaryKeyName, true) == 0)
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
                                AddParam(cmd, i.Value.GetValue(poco), i.Value.PropertyInfo);
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
                                AddParam(cmd, pc.GetValue(poco), pc.PropertyInfo);
                            }

                            // Grab primary key value
                            if (primaryKeyValue == null)
                            {
                                var pc = pd.Columns[primaryKeyName];
                                primaryKeyValue = pc.GetValue(poco);
                            }
                        }

                        // Find the property info for the primary key
                        PropertyInfo pkpi = null;
                        if (primaryKeyName != null)
                        {
                            PocoColumn col;
                            pkpi = pd.Columns.TryGetValue(primaryKeyName, out col) ? col.PropertyInfo : new { Id = primaryKeyValue }.GetType().GetProperties()[0];
                        }

                        cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}", _provider.EscapeTableName(tableName), sb.ToString(),
                            _provider.EscapeSqlIdentifier(primaryKeyName), _paramPrefix, index++);
                        AddParam(cmd, primaryKeyValue, pkpi);

                        DoPreExecute(cmd);

                        // Do it
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
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

#endregion

#region operation: Delete

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object whose primary key value will be used to delete the row</param>
        /// <returns>The number of rows affected</returns>
        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return Delete(tableName, primaryKeyName, poco, null);
        }

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">
        ///     The POCO object whose primary key value will be used to delete the row (or null to use the supplied
        ///     primary key value)
        /// </param>
        /// <param name="primaryKeyValue">
        ///     The value of the primary key identifing the record to be deleted (or null, or get this
        ///     value from the POCO instance)
        /// </param>
        /// <returns>The number of rows affected</returns>
        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null)
            {
                var pd = PocoData.ForObject(poco, primaryKeyName, _defaultMapper);
                PocoColumn pc;
                if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                {
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", _provider.EscapeTableName(tableName), _provider.EscapeSqlIdentifier(primaryKeyName));
            return Execute(sql, primaryKeyValue);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        public int Delete(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        public int Delete<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
                return Delete(pocoOrPrimaryKey);

            var pd = PocoData.ForType(typeof(T), _defaultMapper);

            if (pocoOrPrimaryKey.GetType().Name.Contains("AnonymousType"))
            {
                var pi = pocoOrPrimaryKey.GetType().GetProperty(pd.TableInfo.PrimaryKey);

                if (pi == null)
                    throw new InvalidOperationException(string.Format("Anonymous type does not contain an id for PK column `{0}`.", pd.TableInfo.PrimaryKey));

                pocoOrPrimaryKey = pi.GetValue(pocoOrPrimaryKey, new object[0]);
            }

            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        public int Delete<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(string.Format("DELETE FROM {0} {1}", _provider.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL condition clause identifying the row to delete (ie:
        ///     everything after "UPDATE tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        public int Delete<T>(Sql sql)
        {
            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            return Execute(new Sql(string.Format("DELETE FROM {0}", _provider.EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }

#endregion

#region operation: IsNew

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        public bool IsNew(string primaryKeyName, object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            if (string.IsNullOrEmpty(primaryKeyName))
                throw new ArgumentException("primaryKeyName");

            return IsNew(primaryKeyName, PocoData.ForObject(poco, primaryKeyName, _defaultMapper), poco);
        }

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

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) || !type.IsValueType)
                return pk == null;

            if (type == typeof(string))
                return string.IsNullOrEmpty((string) pk);
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

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        public bool IsNew(object poco)
        {
            if (poco == null)
                throw new ArgumentNullException(nameof(poco));

            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            return IsNew(pd.TableInfo.PrimaryKey, pd, poco);
        }

#endregion

#region operation: Save

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="tableName">The name of the table to be updated</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save(string tableName, string primaryKeyName, object poco)
        {
            if (IsNew(primaryKeyName, poco))
            {
                Insert(tableName, primaryKeyName, true, poco);
            }
            else
            {
                Update(tableName, primaryKeyName, poco);
            }
        }

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        public void Save(object poco)
        {
            var pd = PocoData.ForType(poco.GetType(), _defaultMapper);
            Save(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

#endregion

#region operation: Multi-Poco Query/Fetch

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return Query<T1, T2, TRet>(cb, sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return Query<T1, T2, T3, TRet>(cb, sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return Query<T1, T2, T3, T4, TRet>(cb, sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args)
        {
            return Query<T1, T2, T3, T4, T5, TRet>(cb, sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2) }, cb, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, cb, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return Query<T1, T2, TRet>(cb, sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return Query<T1, T2, T3, TRet>(cb, sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return Query<T1, T2, T3, T4, TRet>(cb, sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The returned list POCO type</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql)
        {
            return Query<T1, T2, T3, T4, T5, TRet>(cb, sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2) }, cb, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql)
        {
            return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, cb, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2>(string sql, params object[] args)
        {
            return Query<T1, T2>(sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3>(string sql, params object[] args)
        {
            return Query<T1, T2, T3>(sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return Query<T1, T2, T3, T4>(sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4, T5>(string sql, params object[] args)
        {
            return Query<T1, T2, T3, T4, T5>(sql, args).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2>(string sql, params object[] args)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5>(string sql, params object[] args)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, null, sql, args);
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2>(Sql sql)
        {
            return Query<T1, T2>(sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3>(Sql sql)
        {
            return Query<T1, T2, T3>(sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4>(Sql sql)
        {
            return Query<T1, T2, T3, T4>(sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco fetch
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as a List</returns>
        public List<T1> Fetch<T1, T2, T3, T4, T5>(Sql sql)
        {
            return Query<T1, T2, T3, T4, T5>(sql.SQL, sql.Arguments).ToList();
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2>(Sql sql)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The fourth POCO type</typeparam>
        /// <typeparam name="T5">The fifth POCO type</typeparam>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Query<T1, T2, T3, T4, T5>(Sql sql)
        {
            return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }, null, sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Performs a multi-poco query
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="types">An array of Types representing the POCO types of the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
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
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
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
                    bool bNeedTerminator = false;
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

#region operation: Multi-Result Set

        /// <summary>
        ///     Perform a multi-results set query
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and it's arguments</param>
        /// <returns>A GridReader to be queried</returns>
        public IGridReader QueryMultiple(Sql sql)
        {
            return QueryMultiple(sql.SQL, sql.Arguments);
        }

        /// <summary>
        ///     Perform a multi-results set query
        /// </summary>
        /// <param name="sql">The SQL query to be executed</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>A GridReader to be queried</returns>
        public IGridReader QueryMultiple(string sql, params object[] args)
        {
            OpenSharedConnection();

            GridReader result = null;

            var cmd = CreateCommand(_sharedConnection, sql, args);

            try
            {
                var reader = cmd.ExecuteReader();
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

#region operation: StoredProc

        /// <summary>
        ///     Runs a stored procedure, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        ///     For any arguments which are POCOs, each readable property will be turned into a named parameter
        ///     for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other
        ///     argument types will throw an exception.
        /// </remarks>
        public IEnumerable<T> QueryProc<T>(string storedProcedureName, params object[] args)
        {
            return ExecuteReader<T>(CommandType.StoredProcedure, storedProcedureName, args);
        }

        /// <summary>
        ///     Runs a stored procedure, returning the results as typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>A List holding the results of the query</returns>
        /// <remarks>
        ///     For any arguments which are POCOs, each readable property will be turned into a named parameter
        ///     for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other
        ///     argument types will throw an exception.
        /// </remarks>
        public List<T> FetchProc<T>(string storedProcedureName, params object[] args) => QueryProc<T>(storedProcedureName, args).ToList();

        /// <summary>
        ///     Executes a stored procedure and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>The scalar value cast to T</returns>
        /// <remarks>
        ///     For any arguments which are POCOs, each readable property will be turned into a named parameter
        ///     for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other
        ///     argument types will throw an exception.
        /// </remarks>
        public T ExecuteScalarProc<T>(string storedProcedureName, params object[] args)
        {
            return ExecuteScalarInternal<T>(CommandType.StoredProcedure, storedProcedureName, args);
        }

        /// <summary>
        ///     Executes a non-query stored procedure
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>The number of rows affected</returns>
        /// <remarks>
        ///     For any arguments which are POCOs, each readable property will be turned into a named parameter
        ///     for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other
        ///     argument types will throw an exception.
        /// </remarks>
        public int ExecuteNonQueryProc(string storedProcedureName, params object[] args)
        {
            return ExecuteInternal(CommandType.StoredProcedure, storedProcedureName, args);
        }

#endregion

#region Last Command

        /// <summary>
        ///     Retrieves the SQL of the last executed statement
        /// </summary>
        public string LastSQL => _lastSql;

        /// <summary>
        ///     Retrieves the arguments to the last execute statement
        /// </summary>
        public object[] LastArgs => _lastArgs;

        /// <summary>
        ///     Returns a formatted string describing the last executed SQL statement and it's argument values
        /// </summary>
        public string LastCommand => FormatCommand(_lastSql, _lastArgs);

#endregion

#region FormatCommand

        /// <summary>
        ///     Formats the contents of a DB command for display
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string FormatCommand(IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        /// <summary>
        ///     Formats an SQL query and it's arguments for display
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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

#region Public Properties

        /// <summary>
        ///     Gets the default mapper.
        /// </summary>
        public IMapper DefaultMapper => _defaultMapper;

        /// <summary>
        ///     When set to true, PetaPoco will automatically create the "SELECT columns" part of any query that looks like it
        ///     needs it
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed in argument values.
        /// </summary>
        public bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value for all SQL statements.
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement
        /// </summary>
        public int OneTimeCommandTimeout { get; set; }

        /// <summary>
        ///     Gets the loaded database provider. <seealso cref="Provider" />.
        /// </summary>
        /// <returns>
        ///     The loaded database type.
        /// </returns>
        public IProvider Provider => _provider;

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>
        ///     The connection string.
        /// </returns>
        public string ConnectionString => _connectionString;

        /// <summary>
        ///     Gets or sets the transaction isolation level.
        /// </summary>
        /// <remarks>
        ///     When value is null, the underlying providers default isolation level is used.
        /// </remarks>
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

#endregion

#region Events

        /// <summary>
        ///     Occurs when a new transaction has started.
        /// </summary>
        public event EventHandler<DbTransactionEventArgs> TransactionStarted;

        /// <summary>
        ///     Occurs when a transaction is about to be rolled back or committed.
        /// </summary>
        public event EventHandler<DbTransactionEventArgs> TransactionEnding;

        /// <summary>
        ///     Occurs when a database command is about to be executed.
        /// </summary>
        public event EventHandler<DbCommandEventArgs> CommandExecuting;

        /// <summary>
        ///     Occurs when a database command has been executed.
        /// </summary>
        public event EventHandler<DbCommandEventArgs> CommandExecuted;

        /// <summary>
        ///     Occurs when a database connection is about to be closed.
        /// </summary>
        public event EventHandler<DbConnectionEventArgs> ConnectionClosing;

        /// <summary>
        ///     Occurs when a database connection has been opened.
        /// </summary>
        public event EventHandler<DbConnectionEventArgs> ConnectionOpened;

        /// <summary>
        ///     Occurs when a database exception has been thrown.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionThrown;

#endregion
    }

    public class Database<TDatabaseProvider> : Database where TDatabaseProvider : IProvider
    {
        /// <summary>
        ///     Constructs an instance using a supplied connection string and provider type.
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