using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    public interface IExecuteAsync
    {
        /// <summary>
        ///     Async version of <see cref="IExecute.Execute(string,object[])" />.
        /// </summary>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(Sql)" />.
        /// </summary>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IExecute.Execute(Sql)" />.
        /// </summary>
        Task<int> ExecuteAsync(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(Sql)" />.
        /// </summary>
        Task<int> ExecuteAsync(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(string,object[])" />.
        /// </summary>
        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(Sql)" />.
        /// </summary>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(Sql)" />.
        /// </summary>
        Task<T> ExecuteScalarAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IExecute.ExecuteScalar{T}(Sql)" />.
        /// </summary>
        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, Sql sql);
    }
#endif
}