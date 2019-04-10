using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IStoredProcAsync
    {
        /// <summary>
        ///     Async version of <see cref="IStoredProc.QueryProc{T}" />.
        /// </summary>
        Task QueryProcAsync<T>(Action<T> receivePocoCallback, string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.QueryProc{T}" />.
        /// </summary>
        Task QueryProcAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.QueryProc{T}" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.QueryProc{T}" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.FetchProc{T}" />.
        /// </summary>
        Task<List<T>> FetchProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.FetchProc{T}" />.
        /// </summary>
        Task<List<T>> FetchProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.ExecuteScalarProc{T}" />.
        /// </summary>
        Task<T> ExecuteScalarProcAsync<T>(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.ExecuteScalarProc{T}" />.
        /// </summary>
        Task<T> ExecuteScalarProcAsync<T>(CancellationToken cancellationToken, string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.ExecuteNonQueryProc" />.
        /// </summary>
        Task<int> ExecuteNonQueryProcAsync(string storedProcedureName, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IStoredProc.ExecuteNonQueryProc" />.
        /// </summary>
        Task<int> ExecuteNonQueryProcAsync(CancellationToken cancellationToken, string storedProcedureName, params object[] args);
    }
}