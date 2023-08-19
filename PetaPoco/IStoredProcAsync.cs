using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Specifies a set of methods for asynchronously executing stored procedures.
    /// </summary>
    public interface IStoredProcAsync
    {
        /// <inheritdoc cref="ExecuteNonQueryProcAsync(CancellationToken, string, object[])"/>
        Task<int> ExecuteNonQueryProcAsync(string storedProcedureName, params object[] args);

        /// <inheritdoc cref="ExecuteScalarProcAsync{T}(CancellationToken, string, object[])"/>
        Task<T> ExecuteScalarProcAsync<T>(string storedProcedureName, params object[] args);

        /// <inheritdoc cref="QueryProcAsync{T}(CancellationToken, string, object[])"/>
        Task<IAsyncReader<T>> QueryProcAsync<T>(string storedProcedureName, params object[] args);

        /// <inheritdoc cref="QueryProcAsync{T}(Action{T}, CancellationToken, string, object[])"/>
        Task QueryProcAsync<T>(Action<T> action, string storedProcedureName, params object[] args);

        /// <inheritdoc cref="FetchProcAsync{T}(CancellationToken, string, object[])"/>
        Task<List<T>> FetchProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a non-query stored procedure and returns the number of rows affected.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the number of rows affected.
        /// </returns>
        Task<int> ExecuteNonQueryProcAsync(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a scalar stored procedure and returns the first column of the first row in the result set.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the scalar result value of type <typeparamref
        /// name="T"/>.
        /// </returns>
        Task<T> ExecuteScalarProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a query stored procedure and returns an IAsyncReader of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the
        /// result set.
        /// </returns>
        Task<IAsyncReader<T>> QueryProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a query stored procedure and invokes the specified action on each POCO in the result set.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task QueryProcAsync<T>(Action<T> action, CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        /// Asynchronously executes a query stored procedure and returns a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// For any arguments which are POCOs, each readable property will be turned into a named parameter for the stored procedure.
        /// Arguments which are IDbDataParameters will be passed through. Any other argument types will throw an exception.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="args">The arguments to pass to the stored procedure.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of POCOs of type <typeparamref name="T"/>.
        /// </returns>
        Task<List<T>> FetchProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);
    }
#endif
}
