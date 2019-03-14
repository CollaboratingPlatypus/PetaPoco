using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IQueryAsync
    {
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandType commandType, CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>();

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType, CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IEnumerableAsyncAdapter<T>> QueryAsyncAdapted<T>(CommandType commandType, CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>();

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(Sql)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, Sql)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CommandType commandType, CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql);
    }
}