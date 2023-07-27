using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Specifies a set of methods for asynchronously performing SQL operations on POCOs such as Insert, Update, Delete, and Save.
    /// </summary>
    public interface IAlterPocoAsync
    {
        #region InsertAsync

        /// <inheritdoc cref="InsertAsync(CancellationToken, object)"/>
        Task<object> InsertAsync(object poco);

        /// <inheritdoc cref="InsertAsync(CancellationToken, string, object)"/>
        Task<object> InsertAsync(string tableName, object poco);

        /// <inheritdoc cref="InsertAsync(CancellationToken, string, string, object)"/>
        Task<object> InsertAsync(string tableName, string primaryKeyName, object poco);

        /// <inheritdoc cref="InsertAsync(CancellationToken, string, string, bool, object)"/>
        Task<object> InsertAsync(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        /// Asynchronously inserts a new record and returns the primary key of the newly inserted record.
        /// </summary>
        /// <inheritdoc cref="InsertAsync(CancellationToken, string, object)"/>
        Task<object> InsertAsync(CancellationToken cancellationToken, object poco);

        /// <remarks>
        /// If a mapped primary key column is auto-incrementing and <see cref="TableInfo.AutoIncrement"/> is <see langword="true"/>, the primary key property of the POCO will be updated with the new record's auto-incremented ID.
        /// </remarks>
        /// <inheritdoc cref="InsertAsync(CancellationToken, string, string, bool, object)"/>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, object poco);

        /// <remarks>
        /// If <paramref name="primaryKeyName"/> represents an auto-incrementing column and <see cref="TableInfo.AutoIncrement"/> is <see langword="true"/>, the primary key property of the POCO will be updated with the new record's auto-incremented ID.
        /// </remarks>
        /// <inheritdoc cref="InsertAsync(CancellationToken, string, string, bool, object)"/>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Asynchronously inserts a new record into the specified table and returns the primary key of the newly inserted record.
        /// </summary>
        /// <remarks>
        /// If <paramref name="autoIncrement"/> is <see langword="true"/>, the primary key property of the POCO will be updated with the new record's auto-incremented ID.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="tableName">The name of the table where the record will be inserted.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="autoIncrement">Specifies whether the primary key column in the database is auto-incrementing.</param>
        /// <param name="poco">The POCO instance to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the primary key of the new record if the table has a primary key column; otherwise, <see langword="null"/>.</returns>
        Task<object> InsertAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, bool autoIncrement, object poco);

        #endregion

        #region UpdateAsync

        /// <inheritdoc cref="UpdateAsync(CancellationToken, object)"/>
        Task<int> UpdateAsync(object poco);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(object poco, IEnumerable<string> columns);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, object, object)"/>
        Task<int> UpdateAsync(object poco, object primaryKeyValue);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object)"/>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object)"/>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <inheritdoc cref="UpdateAsync{T}(CancellationToken, Sql)"/>
        Task<int> UpdateAsync<T>(Sql sql);

        /// <inheritdoc cref="UpdateAsync{T}(CancellationToken, string, object[])"/>
        Task<int> UpdateAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously updates a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        /// Asynchronously updates the specified columns of a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, IEnumerable<string> columns);

        /// <summary>
        /// Asynchronously updates a record with the given ID and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue);

        /// <summary>
        /// Asynchronously updates the specified columns of a record with the given ID and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Asynchronously a record in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Asynchronously updates the specified columns of a record in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        /// Asynchronously a record with the given ID in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="UpdateAsync(CancellationToken, string, string, object, object, IEnumerable{string})"/>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Asynchronously updates the specified columns of a record with the given ID in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance containing the column values to update.</param>
        /// <param name="primaryKeyValue">The primary key value identifying the record to update.</param>
        /// <param name="columns">A list of column names to update, or <see langword="null"/> to update all columns.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the update operation.</returns>
        Task<int> UpdateAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Asynchronously executes an SQL update and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to update.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the condition portion of the WHERE clause identifying the row to update (everything after <c>UPDATE tablename</c>) and its parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the update operation.</returns>
        Task<int> UpdateAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes an SQL update and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to update.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause identifying the row to update (everything after <c>UPDATE tablename</c>).</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the update operation.</returns>
        Task<int> UpdateAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region DeleteAsync

        /// <inheritdoc cref="DeleteAsync(CancellationToken, object)"/>
        Task<int> DeleteAsync(object poco);

        /// <inheritdoc cref="DeleteAsync(CancellationToken, string, string, object)"/>
        Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco);

        /// <inheritdoc cref="DeleteAsync(CancellationToken, string, string, object, object)"/>
        Task<int> DeleteAsync(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <inheritdoc cref="DeleteAsync{T}(CancellationToken, object)"/>
        Task<int> DeleteAsync<T>(object pocoOrPrimaryKeyValue);

        /// <inheritdoc cref="DeleteAsync{T}(CancellationToken, Sql)"/>
        Task<int> DeleteAsync<T>(Sql sql);

        /// <inheritdoc cref="DeleteAsync{T}(CancellationToken, string, object[])"/>
        Task<int> DeleteAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously deletes a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="poco">The POCO instance representing the record to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        /// Asynchronously deletes a record from the provided table and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="tableName">The name of the table containing the record to delete.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance representing the record to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Asynchronously deletes a record with the given ID from the provided table and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="tableName">The name of the table containing the record to delete.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance representing the record to delete, or <see langword="null"/> to use the provided <paramref name="primaryKeyValue"/>.</param>
        /// <param name="primaryKeyValue">The primary key value identifying the record to delete, used if <paramref name="poco"/> is <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Asynchronously deletes a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="pocoOrPrimaryKeyValue">The primary key value, or a POCO containing an assigned primary key value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKeyValue);

        /// <summary>
        /// Asynchronously executes an SQL delete and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the condition portion of the WHERE clause identifying the row to delete (everything after <c>DELETE FROM tablename</c>) and its parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes an SQL delete and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause identifying the row to delete (everything after <c>DELETE FROM tablename</c>).</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected by the delete operation.</returns>
        Task<int> DeleteAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region SaveAsync

        /// <inheritdoc cref="SaveAsync(CancellationToken, object)"/>
        Task SaveAsync(object poco);

        /// <inheritdoc cref="SaveAsync(CancellationToken, string, string, object)"/>
        Task SaveAsync(string tableName, string primaryKeyName, object poco);

        /// <remarks>
        /// Performs an <see cref="InsertAsync(object)"/> operation if the POCO is new (as determined by <see cref="IAlterPoco.IsNew(object)"/>), and an <see cref="UpdateAsync(object)"/> operation otherwise.
        /// <para>If an Insert operation is performed, and a mapped primary key column is auto-incrementing, the primary key property of the POCO will be updated with the new record's auto-incremented ID.</para>
        /// </remarks>
        /// <inheritdoc cref="SaveAsync(CancellationToken, string, string, object)"/>
        Task SaveAsync(CancellationToken cancellationToken, object poco);

        /// <summary>
        /// Asynchronously saves the specified POCO to the database by performing either an insert or an update operation, as appropriate.
        /// </summary>
        /// <remarks>
        /// Performs an <see cref="InsertAsync(string, string, object)"/> operation if the POCO is new (as determined by <see cref="IAlterPoco.IsNew(string, object)"/>), and an <see cref="UpdateAsync(string, string, object)"/> operation otherwise.
        /// <para>If an Insert operation is performed, and <paramref name="primaryKeyName"/> represents an auto-incrementing column, the primary key property of the POCO will be updated with the new record's auto-incremented ID.</para>
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="tableName">The name of the table where the record will be saved.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance to save.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveAsync(CancellationToken cancellationToken, string tableName, string primaryKeyName, object poco);

        #endregion
    }
#endif
}
