using System;
// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/11/03</date>

using System;
using System.Collections.Generic;
using System.Data;
using PetaPoco.Core;

namespace PetaPoco
{
    public interface IStoredProc
    {
        /// <summary>
        /// Runs a stored procedure, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>An enumerable collection of result records</returns>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter
        /// for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other 
        /// argument types will throw an exception.
        /// </remarks>
        IEnumerable<T> QueryProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Runs a stored procedure, returning the results as typed list
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>A List holding the results of the query</returns>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter
        /// for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other 
        /// argument types will throw an exception.
        /// </remarks>
        List<T> FetchProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Executes a stored procedure and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>The scalar value cast to T</returns>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter
        /// for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other 
        /// argument types will throw an exception.
        /// </remarks>
        T ExecuteScalarProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Executes a non-query stored procedure
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to run</param>
        /// <param name="args">Arguments for the stored procedure</param>
        /// <returns>The number of rows affected</returns>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter
        /// for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other 
        /// argument types will throw an exception.
        /// </remarks>
        int ExecuteNonQueryProc(string storedProcedureName, params object[] args);
    }
}
