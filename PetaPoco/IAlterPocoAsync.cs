using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IAlterPocoAsync
    {
        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, object)" />.
        /// </summary>
        Task<object> InsertAsync(string tableName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, object)" />.
        /// </summary>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, string, object)" />.
        /// </summary>
        Task<object> InsertAsync(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, string, object)" />.
        /// </summary>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, string, bool, object)" />.
        /// </summary>
        Task<object> InsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(string, string, bool, object)" />.
        /// </summary>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(object)" />.
        /// </summary>
        Task<object> InsertAsync(object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Insert(object)" />.
        /// </summary>
        Task<object> InsertAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, object)" />.
        /// </summary>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, object)" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue,
                              IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object)" />.
        /// </summary>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object)" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(string, string, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object)" />.
        /// </summary>
        Task<int> UpdateAsync(object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object)" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, object)" />.
        /// </summary>
        Task<int> UpdateAsync(object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, object)" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update(object, object, IEnumerable{string})" />.
        /// </summary>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update{t}(string, object[])" />.
        /// </summary>
        Task<int> UpdateAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update{t}(string, object[])" />.
        /// </summary>
        Task<int> UpdateAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update{t}(Sql)" />.
        /// </summary>
        Task<int> UpdateAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Update{t}(Sql)" />.
        /// </summary>
        Task<int> UpdateAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(string, string, object)" />.
        /// </summary>
        Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(string, string, object)" />.
        /// </summary>
        Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(string, string, object, object)" />.
        /// </summary>
        Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(string, string, object, object)" />.
        /// </summary>
        Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(object)" />.
        /// </summary>
        Task<int> DeleteAsync(object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete(object)" />.
        /// </summary>
        Task<int> DeleteAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(object)" />.
        /// </summary>
        Task<int> DeleteAsync<T>(object pocoOrPrimaryKey);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(object)" />.
        /// </summary>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKey);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(string, object[])" />.
        /// </summary>
        Task<int> DeleteAsync<T>(string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(string, object[])" />.
        /// </summary>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(Sql)" />.
        /// </summary>
        Task<int> DeleteAsync<T>(Sql sql);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Delete{t}(Sql)" />.
        /// </summary>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Save(string, string, object)" />.
        /// </summary>
        Task SaveAsync(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Save(string, string, object)" />.
        /// </summary>
        Task SaveAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Save(object)" />.
        /// </summary>
        Task SaveAsync(object poco);

        /// <summary>
        ///     Async version of <see cref="IAlterPoco.Save(object)" />.
        /// </summary>
        Task SaveAsync(CancellationToken cancellationToken, object poco);
    }
}