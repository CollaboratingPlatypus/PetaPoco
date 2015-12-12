// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/12</date>

using System;
using PetaPoco.Internal;

namespace PetaPoco
{
    public interface IDatabase : IDisposable, IQuery, IAlterPoco, IExecute
    {
        /// <summary>
        ///     Retrieves the SQL of the last executed statement
        /// </summary>
        string LastSQL { get; }

        /// <summary>
        ///     Retrieves the arguments to the last execute statement
        /// </summary>
        object[] LastArgs { get; }

        /// <summary>
        ///     Returns a formatted string describing the last executed SQL statement and it's argument values
        /// </summary>
        string LastCommand { get; }

        /// <summary>
        ///     When set to true, PetaPoco will automatically create the "SELECT columns" part of any query that looks like it
        ///     needs it
        /// </summary>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        ///     When set to true, parameters can be named ?myparam and populated from properties of the passed in argument values.
        /// </summary>
        bool EnableNamedParams { get; set; }

        /// <summary>
        ///     Sets the timeout value for all SQL statements.
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        ///     Sets the timeout value for the next (and only next) SQL statement
        /// </summary>
        int OneTimeCommandTimeout { get; set; }

        /// <summary>
        ///     Gets the loaded database type. <seealso cref="Provider" />.
        /// </summary>
        /// <returns>
        ///     The loaded database type.
        /// </returns>
        IProvider Provider { get; }
    }
}