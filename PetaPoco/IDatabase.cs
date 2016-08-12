// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/21</date>

using System;
using System.Data;
using PetaPoco.Core;

namespace PetaPoco
{
    /// <summary>
    ///     Specifies the database contract.
    /// </summary>
    public interface IDatabase : IDisposable, IQuery, IAlterPoco, IExecute, ITransactionAccessor
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
        ///     Gets a formatted string describing the last executed SQL statement and it's argument values
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
        ///     True, if auto select is enabled; Else, false.
        /// </returns>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     Gets the flag for whether named params are enabled. (Default is True)
        /// </summary>
        /// <remarks>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed in argument values.
        /// </remarks>
        /// <returns>
        ///     True, if named parameters are enabled; Else, false.
        /// </returns>
        bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value, in seconds, which PetaPoco applies to all <see cref="IDbCommand.CommandTimeout" />.
        ///     (Default is 0)
        /// </summary>
        /// <remarks>
        ///     If the current value is zero PetaPoco will not set the command timeout, and therefor, the .net default (30 seconds)
        ///     will be in affect.
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
        ///     When value is null, the underlying providers default isolation level is used.
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
        ///     Aborts the entire outer most transaction scope
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
    }
}