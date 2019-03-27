using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
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
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType);

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
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

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
        Task QueryAsync<T>(Action<T> receivePocoCallback, CancellationToken cancellationToken, CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>();

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}()" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(string, object[])" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Query{T}(Sql)" />.
        /// </summary>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql);

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
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType);

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
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

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
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,string,object[],string,object[])" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,string,object[],string,object[])" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,string,object[])" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,string,object[])" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,Sql)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,Sql)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,Sql,Sql)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Page{T}(long,long,Sql,Sql)" />.
        /// </summary>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long itemsPerPage, Sql sqlCount, Sql sqlPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long)" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long)" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long,string,object[])" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long,string,object[])" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long,Sql)" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.SkipTake{T}(long,long,Sql)" />.
        /// </summary>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Exists{T}(object)" />.
        /// </summary>
        Task<bool> ExistsAsync<T>(object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.Exists{T}(object)" />.
        /// </summary>
        Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.Exists{T}(string, object[])" />.
        /// </summary>
        Task<bool> ExistsAsync<T>(string sqlCondition, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Exists{T}(string, object[])" />.
        /// </summary>
        Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, string sqlCondition, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(object)" />.
        /// </summary>
        Task<T> SingleAsync<T>(object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(object)" />.
        /// </summary>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(string, object[])" />.
        /// </summary>
        Task<T> SingleAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(string, object[])" />.
        /// </summary>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(Sql)" />.
        /// </summary>
        Task<T> SingleAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Single{T}(CancellationToken, Sql)" />.
        /// </summary>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(PetaPoco.Sql)" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(PetaPoco.Sql)" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(object)" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(object)" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, object primaryKey);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(string, object[])" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.SingleOrDefault{T}(string, object[])" />.
        /// </summary>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.First{T}(string,object[])" />.
        /// </summary>
        Task<T> FirstAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.First{T}(string,object[])" />.
        /// </summary>
        Task<T> FirstAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.First{T}(Sql)" />.
        /// </summary>
        Task<T> FirstAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.First{T}(Sql)" />.
        /// </summary>
        Task<T> FirstAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.FirstOrDefault{T}(string,object[])" />.
        /// </summary>
        Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.FirstOrDefault{T}(string,object[])" />.
        /// </summary>
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.FirstOrDefault{T}(Sql)" />.
        /// </summary>
        Task<T> FirstOrDefaultAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.FirstOrDefault{T}(Sql)" />.
        /// </summary>
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql);
    }
#endif
}