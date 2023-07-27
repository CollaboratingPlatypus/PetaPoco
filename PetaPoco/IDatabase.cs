using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;

namespace PetaPoco
{
    /// <summary>
    /// Represents the core functionality of PetaPoco.
    /// </summary>
    public interface IDatabase : IDisposable, IQuery, IAlterPoco, IExecute, ITransactionAccessor, IStoredProc, IConnection
#if ASYNC
                                 , IQueryAsync, IExecuteAsync, IStoredProcAsync, IAlterPocoAsync
#endif
    {
        /// <summary>
        /// Gets the default mapper.
        /// </summary>
        IMapper DefaultMapper { get; }

        /// <summary>
        /// Gets the SQL of the last executed command.
        /// </summary>
        string LastSQL { get; }

        /// <summary>
        /// Gets an array containing the arguments of the last executed command.
        /// </summary>
        object[] LastArgs { get; }

        /// <summary>
        /// Gets a formatted string describing the last executed command and its argument values.
        /// </summary>
        string LastCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic generation of the <c>SELECT</c> and <c>WHERE</c> parts of an SQL statement is enabled when not explicitly provided by the caller.
        /// </summary>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether named parameters are enabled.
        /// </summary>
        bool EnableNamedParams { get; set; }

        /// <summary>
        /// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets a single-use timeout value that will temporarily override <see cref="CommandTimeout"/> for the next command execution.
        /// </summary>
        int OneTimeCommandTimeout { get; set; }

        /// <summary>
        /// Gets the underlying database Provider.
        /// </summary>
        IProvider Provider { get; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        string ConnectionString { get; }

        #region Transaction Interface

        /// <summary>
        /// Gets or sets the transaction isolation level.
        /// </summary>
        IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Begins or continues a transaction.
        /// </summary>
        ITransaction GetTransaction();

        /// <summary>
        /// Begins a transaction scope.
        /// </summary>
        void BeginTransaction();

#if ASYNC
        /// <summary>
        /// Asynchronously begins a transaction scope.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Asynchronously begins a transaction scope.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken);
#endif

        /// <summary>
        /// Aborts the entire outermost transaction scope.
        /// </summary>
        void AbortTransaction();

#if ASYNC
        // TODO: Missing: `AbortTransactionAsync()`
#endif

        /// <summary>
        /// Marks the current transaction scope as complete.
        /// </summary>
        void CompleteTransaction();

#if ASYNC
        // TODO: Missing: `CompleteTransactionAsync()`
#endif

        /// <summary>
        /// Occurs when a new transaction has started.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionStarted;

        /// <summary>
        /// Occurs when a transaction is about to be rolled back or committed.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionEnding;

        #endregion

        /// <summary>
        /// Occurs when a database command is about to be executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when a database command has been executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuted;

        // TODO: Missing: `event EventHandler<DbConnectionEventArgs> ConnectionOpening`

        /// <summary>
        /// Occurs when a database connection has been opened.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionOpened;

        /// <summary>
        /// Occurs when a database connection is about to be closed.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionClosing;

        /// <summary>
        /// Occurs when a database exception has been thrown.
        /// </summary>
        event EventHandler<ExceptionEventArgs> ExceptionThrown;
    }
}
