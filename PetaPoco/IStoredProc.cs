using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for executing stored procedures.
    /// </summary>
    public interface IStoredProc
    {
        /// <summary>
        /// Executes a non-query stored procedure and returns the number of rows affected.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQueryProc(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a scalar stored procedure and returns the first column of the first row in the result set.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>The scalar result value of type <typeparamref name="T"/>.</returns>
        T ExecuteScalarProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a query stored procedure and returns the results as a sequence of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence of results.</returns>
        IEnumerable<T> QueryProc<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Executes a query stored procedure and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A <see cref="List{T}"/> containing the results.</returns>
        List<T> FetchProc<T>(string storedProcedureName, params object[] args);
    }
}
