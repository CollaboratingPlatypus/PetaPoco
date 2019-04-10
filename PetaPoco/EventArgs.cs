using System;
using System.Data;

namespace PetaPoco
{
    public class DbTransactionEventArgs : EventArgs
    {
        public IDbTransaction Transaction { get; }

        public DbTransactionEventArgs(IDbTransaction transaction)
        {
            Transaction = transaction;
        }
    }

    public class DbCommandEventArgs : EventArgs
    {
        public IDbCommand Command { get; }

        public DbCommandEventArgs(IDbCommand cmd)
        {
            Command = cmd;
        }
    }

    public class DbConnectionEventArgs : EventArgs
    {
        public IDbConnection Connection { get; set; }

        public DbConnectionEventArgs(IDbConnection connection)
        {
            Connection = connection;
        }
    }

    /// <inheritdoc />
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        ///     A flag which specifies whether the exception should be raised or ignored.
        /// </summary>
        public bool Raise { get; set; } = true;

        /// <summary>
        ///     The exception which was caught.
        /// </summary>
        public Exception Exception { get; }

        public ExceptionEventArgs(Exception ex)
            => Exception = ex;
    }
}