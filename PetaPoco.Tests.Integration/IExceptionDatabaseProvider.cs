using System;
using System.Collections.Generic;
using System.Data;

namespace PetaPoco.Tests.Integration
{
    public interface IExceptionDatabaseProvider
    {
        bool ThrowExceptions { get; set; }
        List<IDataParameter> Parameters { get; set; }

        void PreExecute(IDbCommand cmd);
    }

    public class PreExecuteException : Exception { }
}
