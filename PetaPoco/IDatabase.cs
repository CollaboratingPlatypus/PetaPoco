using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;

namespace PetaPoco
{
    // TODO: Comments stating default values should be moved to the base implementation, where the defaults are actually set.
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
        /// <remarks>
        /// By default, this is the <see cref="ConventionMapper" />.
        /// </remarks>
        /// <returns>The default mapper.</returns>
        IMapper DefaultMapper { get; }

        /// <summary>
        /// Gets the SQL of the last executed statement.
        /// </summary>
        /// <returns>The last executed SQL.</returns>
        string LastSQL { get; }

        /// <summary>
        /// Gets the arguments to the last execute statement.
        /// </summary>
        /// <returns>The last executed SQL arguments.</returns>
        object[] LastArgs { get; }

        /// <summary>
        /// Gets a formatted string describing the last executed SQL statement and its argument values.
        /// </summary>
        /// <returns>The formatted string.</returns>
        string LastCommand { get; }

        /// <summary>
        /// Gets or sets the enable auto select. The default value is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/>, PetaPoco will automatically create the "SELECT columns" section of the query for any query which is found to require them.
        /// </remarks>
        /// <returns><see langword="true"/>, if auto select is enabled; otherwise, <see langword="false"/>.</returns>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Gets or sets the flag for whether named params are enabled. The default value is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/>, parameters can be named ?myparam and populated from properties of the passed-in argument values.
        /// </remarks>
        /// <returns><see langword="true"/>, if named parameters are enabled; otherwise, <see langword="false"/>.</returns>
        bool EnableNamedParams { get; set; }

        /// <summary>
        /// Gets or sets the command timeout value, in seconds, which PetaPoco applies to all commands. The default value is 0 seconds.
        /// </summary>
        /// <remarks>
        /// If the current value is zero PetaPoco will not set the command timeout, and therefore the .NET default (30 seconds) will be in effect.
        /// </remarks>
        /// <returns>The current command timeout.</returns>
        int CommandTimeout { get; set; }

        /// <summary>
        /// Gets or sets the timeout value for the next (and only next) SQL statement.
        /// </summary>
        /// <remarks>
        /// This is a one-shot setting, which after use, will revert back to the previously set <see cref="CommandTimeout" /> setting.
        /// </remarks>
        /// <returns>The one time command timeout.</returns>
        int OneTimeCommandTimeout { get; set; }

        /// <summary>
        /// Gets the current database Provider.
        /// </summary>
        /// <returns>The current database provider.</returns>
        IProvider Provider { get; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        string ConnectionString { get; }

        /// <summary>
        /// Gets or sets the transaction isolation level.
        /// </summary>
        /// <value>If <see langword="null"/>, the underlying provider's default isolation level is used.</value>
        IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Starts or continues a transaction.
        /// </summary>
        /// <remarks>
        /// This method facilitates proper transaction lifetime management, especially when nested.
        /// <para/>Transactions can be nested but they must all be <see cref="CompleteTransaction">completed</see> prior to leaving their scope, otherwise the entire transaction is aborted.
        /// <example>
        /// A basic example of using transactional scopes (part pseudocode) is shown below:
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
        /// </remarks>
        /// <returns>An <see cref="ITransaction" /> reference that must be <see cref="CompleteTransaction">completed</see> or <see cref="IDisposable.Dispose">disposed</see>.</returns>
        ITransaction GetTransaction();

        /// <summary>
        /// Starts a transaction scope.
        /// </summary>
        /// <remarks>
        /// <example>
        /// <inheritdoc cref="GetTransaction"/>
        /// </example>
        /// </remarks>
        void BeginTransaction();

        /// <summary>
        /// Aborts the entire outermost transaction scope.
        /// </summary>
        /// <remarks>
        /// Called automatically by <see cref="IDisposable.Dispose"/> if the transaction wasn't completed.
        /// </remarks>
        void AbortTransaction();

        /// <summary>
        /// Marks the current transaction scope as complete.
        /// </summary>
        /// <remarks>
        /// <example>
        /// <inheritdoc cref="GetTransaction"/>
        /// </example>
        /// </remarks>
        void CompleteTransaction();

        /// <summary>
        /// Occurs when a new transaction has started.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionStarted;

        /// <summary>
        /// Occurs when a transaction is about to be rolled back or committed.
        /// </summary>
        event EventHandler<DbTransactionEventArgs> TransactionEnding;

        /// <summary>
        /// Occurs when a database command is about to be executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when a database command has been executed.
        /// </summary>
        event EventHandler<DbCommandEventArgs> CommandExecuted;

        /// <summary>
        /// Occurs when a database connection is about to be closed.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionClosing;

        /// <summary>
        /// Occurs when a database connection has been opened.
        /// </summary>
        event EventHandler<DbConnectionEventArgs> ConnectionOpened;

        /// <summary>
        /// Occurs when a database exception has been thrown.
        /// </summary>
        event EventHandler<ExceptionEventArgs> ExceptionThrown;

#if ASYNC
        /// <summary>
        /// Async version of <see cref="BeginTransaction" />.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Async version of <see cref="BeginTransaction" />.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken);
#endif
    }
}
