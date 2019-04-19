using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;

namespace PetaPoco
{
    /// <summary>
    ///     Represents the core functionality of PetaPoco.
    /// </summary>
    public interface IDatabase : IDisposable, IQuery, IAlterPoco, IExecute, ITransactionAccessor, IStoredProc, IConnection
#if ASYNC
                                 , IQueryAsync, IExecuteAsync, IStoredProcAsync, IAlterPocoAsync
#endif
    {
        /// <summary>
        ///     Gets the default mapper. (Default is <see cref="ConventionMapper" />)
        /// </summary>
        /// <returns>
        ///     The default mapper.
        /// </returns>
        IMapper DefaultMapper { get; }

        /// <summary>
        ///     Gets the SQL of the last executed statement
        /// </summary>
        /// <returns>
        ///     The last executed SQL.
        /// </returns>
        string LastSQL { get; }

        /// <summary>
        ///     Gets the arguments to the last execute statement
        /// </summary>
        /// <returns>
        ///     The last executed SQL arguments.
        /// </returns>
        object[] LastArgs { get; }

        /// <summary>
        ///     Gets a formatted string describing the last executed SQL statement and its argument values
        /// </summary>
        /// <returns>
        ///     The formatted string.
        /// </returns>
        string LastCommand { get; }

        /// <summary>
        ///     Gets or sets the enable auto select. (Default is True)
        /// </summary>
        /// <remarks>
        ///     When set to true, PetaPoco will automatically create the "SELECT columns" section of the query for any query which
        ///     is found to require them.
        /// </remarks>
        /// <returns>
        ///     True, if auto select is enabled; else, false.
        /// </returns>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     Gets the flag for whether named params are enabled. (Default is True)
        /// </summary>
        /// <remarks>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed-in argument values.
        /// </remarks>
        /// <returns>
        ///     True, if named parameters are enabled; else, false.
        /// </returns>
        bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value, in seconds, which PetaPoco applies to all <see cref="IDbCommand.CommandTimeout" />.
        ///     (Default is 0)
        /// </summary>
        /// <remarks>
        ///     If the current value is zero PetaPoco will not set the command timeout, and therefore the .NET default (30 seconds)
        ///     will be in effect.
        /// </remarks>
        /// <returns>
        ///     The current command timeout.
        /// </returns>
        int CommandTimeout { get; set; }

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        /// <remarks>
        ///     This is a one-time settings, which after use, will return the <see cref="CommandTimeout" /> setting.
        /// </remarks>
        /// <returns>
        ///     The one time command timeout.
        /// </returns>
        int OneTimeCommandTimeout { get; set; }

        /// <summary>
        ///     Gets the current <seealso cref="Provider" />.
        /// </summary>
        /// <returns>
        ///     The current database provider.
        /// </returns>
        IProvider Provider { get; }

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        /// <returns>
        ///     The connection string.
        /// </returns>
        string ConnectionString { get; }

        /// <summary>
        ///     Gets or sets the transaction isolation level.
        /// </summary>
        /// <remarks>
        ///     When value is null, the underlying provider's default isolation level is used.
        /// </remarks>
        IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        ///     Starts or continues a transaction.
        /// </summary>
        /// <returns>An ITransaction reference that must be Completed or disposed</returns>
        /// <remarks>
        ///     This method makes management of calls to Begin/End/CompleteTransaction easier.
        ///     The usage pattern for this should be:
        ///     using (var tx = db.GetTransaction())
        ///     {
        ///     // Do stuff
        ///     db.Update(...);
        ///     // Mark the transaction as complete
        ///     tx.Complete();
        ///     }
        ///     Transactions can be nested but they must all be completed otherwise the entire
        ///     transaction is aborted.
        /// </remarks>
        ITransaction GetTransaction();

        /// <summary>
        ///     Starts a transaction scope, see GetTransaction() for recommended usage
        /// </summary>
        void BeginTransaction();

        /// <summary>
        ///     Aborts the entire outermost transaction scope
        /// </summary>
        /// <remarks>
        ///     Called automatically by Transaction.Dispose()
        ///     if the transaction wasn't completed.
        /// </remarks>
        void AbortTransaction();

        /// <summary>
        ///     Marks the current transaction scope as complete.
        /// </summary>
        void CompleteTransaction();

        /// <summary>
        ///     Occurs when a new transaction has started.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionStarted;

        /// <summary>
        ///     Occurs when a transaction is about to be rolled back or committed.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionEnding;

        /// <summary>
        ///     Occurs when a database command is about to be executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuting;

        /// <summary>
        ///     Occurs when a database command has been executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuted;

        /// <summary>
        ///     Occurs when a database connection is about to be closed.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionClosing;

        /// <summary>
        ///     Occurs when a database connection has been opened.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionOpened;

        /// <summary>
        ///     Occurs when a database exception has been thrown.
        /// </summary>
        event EventHandler<ExceptionEventArgs> ExceptionThrown;

#if ASYNC
        /// <summary>
        ///     Async version of <see cref="BeginTransaction" />.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        ///     Async version of <see cref="BeginTransaction" />.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken);
#endif
    }
}