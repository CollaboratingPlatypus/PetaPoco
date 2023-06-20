using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Defines an interface for asynchronously executing SQL commands and queries.
    /// </summary>
    /// <remarks>
    /// This interface provides asynchronous methods for executing SQL non-query commands and scalar queries. It supports SQL commands and queries represented as strings or as <see cref="Sql">Sql Builder</see> objects.
    /// </remarks>
    public interface IExecuteAsync
    {
        /// <inheritdoc cref="ExecuteAsync(CancellationToken, string, object[])"/>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes a non-query command with a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected.</returns>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args);

        /// <inheritdoc cref="ExecuteAsync(CancellationToken, Sql)"/>
        Task<int> ExecuteAsync(Sql sql);

        /// <summary>
        /// Asynchronously executes a non-query command with a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An Sql builder object representing the SQL statement and its arguments.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected.</returns>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql);

        /// <inheritdoc cref="ExecuteScalarAsync(CancellationToken, string, object[])"/>
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes the query with a cancellation token and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the scalar value cast to <typeparamref name="T"/>.</returns>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <inheritdoc cref="ExecuteScalarAsync(CancellationToken, Sql)"/>
        Task<T> ExecuteScalarAsync<T>(Sql sql);

        /// <summary>
        /// Asynchronously executes the query with a cancellation token and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An Sql builder object representing the SQL query and its arguments.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the scalar value cast to <typeparamref name="T"/>.</returns>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, Sql sql);
    }
#endif
}
