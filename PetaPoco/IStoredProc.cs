using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Defines an interface for executing stored procedures.
    /// </summary>
    public interface IStoredProc
    {
        /// <summary>
        /// Executes a non-query stored procedure and returns the number of rows affected.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>The number of rows affected by the stored procedure.</returns>
        int ExecuteNonQueryProc(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a stored procedure and returns the first column of the first row in the result set as type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>The scalar result of the stored procedure of type T.</returns>
        T ExecuteScalarProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a stored procedure and returns the result as an IEnumerable of type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>An IEnumerable of type T containing the result set of the stored procedure.</returns>
        IEnumerable<T> QueryProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a stored procedure and returns the result as a List of type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A List of type T containing the result set of the stored procedure.</returns>
        List<T> FetchProc<T>(string storedProcedureName, params object[] args);
    }
}
