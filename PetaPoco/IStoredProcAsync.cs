using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Defines an interface for asynchronously executing stored procedures.
    /// </summary>
    public interface IStoredProcAsync
    {
        /// <inheritdoc cref="QueryProcAsync{T}(Action{T}, CancellationToken, string, object[])"/>
        Task QueryProcAsync<T>(Action<T> receivePocoCallback, string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the result as an IAsyncReader of type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="receivePocoCallback">An action callback to execute on each POCO in the result set.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A task representing the asynchronous operation. The task result is an IAsyncReader of type T containing the result set of the stored procedure.</returns>
        Task QueryProcAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <inheritdoc cref="QueryProcAsync{T}(CancellationToken, string, object[])"/>
        Task<IAsyncReader<T>> QueryProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the result as an IAsyncReader of type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A task representing the asynchronous operation. The task result is an IAsyncReader of type T containing the result set of the stored procedure.</returns>
        Task<IAsyncReader<T>> QueryProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <inheritdoc cref="FetchProcAsync{T}(CancellationToken, string, object[])"/>
        Task<List<T>> FetchProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the result as a List of type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a List of type T containing the result set of the stored procedure.</returns>
        Task<List<T>> FetchProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <inheritdoc cref="ExecuteScalarProcAsync{T}(CancellationToken, string, object[])"/>
        Task<T> ExecuteScalarProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a stored procedure and returns the first column of the first row in the result set as type T.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A task representing the asynchronous operation. The task result is the scalar result of the stored procedure of type T.</returns>
        Task<T> ExecuteScalarProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <inheritdoc cref="ExecuteNonQueryProcAsync(CancellationToken, string, object[])"/>
        Task<int> ExecuteNonQueryProcAsync(string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a non-query stored procedure and returns the number of rows affected.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure. Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>A task representing the asynchronous operation. The task result is the number of rows affected by the stored procedure.</returns>
        Task<int> ExecuteNonQueryProcAsync(CancellationToken cancellationToken, string storedProcedureName, params object[] args);
    }
#endif
}
