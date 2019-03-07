using System;
using System.Data;
using System.Data.Common;

namespace PetaPoco
{
    public class DbTransactionEventArgs: EventArgs
    {
        public IDbTransaction Transaction { get; }

        public DbTransactionEventArgs(IDbTransaction transaction)
        {
            Transaction = transaction;
        }
    }

    public class DbCommandEventArgs: EventArgs
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

    public class ExceptionEventArgs : EventArgs
    {
        public bool Raise { get; set; } = true;

        public Exception Exception { get; }

        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
