using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IQueryAsync
    {
        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}()" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>();

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(Sql)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, string, object[])" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IQuery.Fetch{T}(long, long, Sql)" />.
        /// </summary>
        Task<List<T>> FetchAsync<T>(long page, long itemsPerPage, Sql sql);
    }
}
