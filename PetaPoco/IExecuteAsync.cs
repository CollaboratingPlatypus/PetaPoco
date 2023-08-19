using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Specifies a set of methods for asynchronously executing SQL non-query commands and scalar queries.
    /// </summary>
    public interface IExecuteAsync
    {
        /// <inheritdoc cref="ExecuteAsync(CancellationToken, Sql)"/>
        Task<int> ExecuteAsync(Sql sql);

        /// <inheritdoc cref="ExecuteAsync(CancellationToken, string, object[])"/>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <inheritdoc cref="ExecuteScalarAsync(CancellationToken, Sql)"/>
        Task<T> ExecuteScalarAsync<T>(Sql sql);

        /// <inheritdoc cref="ExecuteScalarAsync(CancellationToken, string, object[])"/>
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes a non-query command and returns the number of rows affected by the operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL statement and its parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is the number of rows affected by the operation.
        /// </returns>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes a non-query command and returns the number of rows affected by the operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is the number of rows affected by the operation.
        /// </returns>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes a scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL statement and its parameters.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is the scalar result value of type <typeparamref name="T"/>.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes a scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL query string.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is the scalar result value of type <typeparamref name="T"/>.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);
    }
#endif
}
