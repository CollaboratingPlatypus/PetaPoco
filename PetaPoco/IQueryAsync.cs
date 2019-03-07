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
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior, CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, string sql, params object[] args);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior, string sql, params object[] args);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, string sql, params object[] args);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior, CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, Sql sql);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior, Sql sql);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, Sql sql);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task QueryAsync<T>(Action<T> receivePocoCallback, CommandBehavior behavior, CancellationToken cancellationToken, Sql sql);
        
        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>();

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(string, object[])" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(Sql)" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, Sql)" />.
        /// </summary>
        //Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql);
    }
}
