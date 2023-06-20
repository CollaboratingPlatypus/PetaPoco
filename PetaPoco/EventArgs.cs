using System;
using System.Data;

namespace PetaPoco
{
    /// <summary>
    /// Event arguments for an <see cref="IDbTransaction"/> event.
    /// </summary>
    public class DbTransactionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the database transaction associated with the event.
        /// </summary>
        public IDbTransaction Transaction { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTransactionEventArgs"/> class.
        /// </summary>
        /// <param name="transaction">The database transaction associated with the event.</param>
        public DbTransactionEventArgs(IDbTransaction transaction) => Transaction = transaction;
    }

    /// <summary>
    /// Event arguments for an <see cref="IDbCommand"/> event.
    /// </summary>
    public class DbCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the database command associated with the event.
        /// </summary>
        public IDbCommand Command { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The database command associated with the event.</param>
        public DbCommandEventArgs(IDbCommand command) => Command = command;
    }

    /// <summary>
    /// Event arguments for an <see cref="IDbConnection"/> event.
    /// </summary>
    public class DbConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the database connection associated with the event.
        /// </summary>
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The database connection associated with the event.</param>
        public DbConnectionEventArgs(IDbConnection connection) => Connection = connection;
    }

    /// <summary>
    /// Event arguments for an <see cref="System.Exception"/> event.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a flag specifying whether the exception should be raised or ignored. Default value is <see langword="true"/>.
        /// </summary>
        public bool Raise { get; set; } = true;

        /// <summary>
        /// Gets the caught exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="ex">The caught exception.</param>
        public ExceptionEventArgs(Exception ex) => Exception = ex;
    }
}
