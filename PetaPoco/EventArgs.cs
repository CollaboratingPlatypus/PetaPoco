using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco
{
    public class DbTransactionEventArgs: EventArgs
    {
        public IDbTransaction Transaction { get; private set; }

        public DbTransactionEventArgs(IDbTransaction transaction)
        {
            Transaction = transaction;
        }
    }

    public class DbCommandEventArgs: EventArgs
    {
        public IDbCommand Command { get; private set; }

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

    public class ExceptionEventArgs: EventArgs
    {
        public bool Raise { get; set; } = true;
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
