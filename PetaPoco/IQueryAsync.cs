using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PetaPoco
{
#if ASYNC
    /// <summary>
    /// Specifies a set of methods for asynchronously executing SQL queries and returning the result set as lists, enumerables, single POCOs, multi-POCOs, or paged results.
    /// </summary>
    public interface IQueryAsync
    {
        #region QueryAsync : Single-POCO

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken)"/>
        Task<IAsyncReader<T>> QueryAsync<T>();

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken, Sql)"/>
        Task<IAsyncReader<T>> QueryAsync<T>(Sql sql);

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken, string, object[])"/>
        Task<IAsyncReader<T>> QueryAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) and returns an async reader.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously executes a query and returns an async reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes a query and returns an async reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region QueryAsync : Single-POCO as CommandType

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken, CommandType)"/>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType);

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken, CommandType, Sql)"/>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, Sql sql);

        /// <inheritdoc cref="QueryAsync{T}(CancellationToken, CommandType, string, object[])"/>
        Task<IAsyncReader<T>> QueryAsync<T>(CommandType commandType, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for the specified command type and returns an async reader.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType);

        /// <summary>
        /// Asynchronously executes a query for the specified command type and returns an async reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql);

        /// <summary>
        /// Asynchronously executes a query for the specified command type and returns an async reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IAsyncReader{T}"/> for reading the result set.</returns>
        Task<IAsyncReader<T>> QueryAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

        #endregion

        #region QueryAsync with Action : Single-POCO

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken)"/>
        Task QueryAsync<T>(Action<T> action);

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, Sql)"/>
        Task QueryAsync<T>(Action<T> action, Sql sql);

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, string, object[])"/>
        Task QueryAsync<T>(Action<T> action, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) and invokes the specified action on each result read from the underlying data reader.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, CommandType, string, object[])"/>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously executes a query and invokes the specified action on each result read from the underlying data reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region QueryAsync with Action : Single-POCO as CommandType

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, CommandType)"/>
        Task QueryAsync<T>(Action<T> action, CommandType commandType);

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, CommandType, Sql)"/>
        Task QueryAsync<T>(Action<T> action, CommandType commandType, Sql sql);

        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, CommandType, string, object[])"/>
        Task QueryAsync<T>(Action<T> action, CommandType commandType, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for the specified command type and invokes the specified action on each result read from the underlying data reader.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="QueryAsync{T}(Action{T}, CancellationToken, CommandType, string, object[])"/>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType, Sql sql);

        /// <summary>
        /// Asynchronously executes a query for the specified command type and invokes the specified action on each result read from the underlying data reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="action">An action to perform on each POCO in the result set.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task QueryAsync<T>(Action<T> action, CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

        #endregion

        #region QueryAsync : Multi-POCO

        //...

        #endregion

        #region QueryMultipleAsync : Multi-POCO Result Set

        //...

        #endregion

        #region FetchAsync : Single-POCO

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken)"/>
        Task<List<T>> FetchAsync<T>();

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, Sql)"/>
        Task<List<T>> FetchAsync<T>(Sql sql);

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, string, object[])"/>
        Task<List<T>> FetchAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, string, object[])"/>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Executes a query and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region FetchAsync : Single-POCO as CommandType

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, CommandType)"/>
        Task<List<T>> FetchAsync<T>(CommandType commandType);

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, CommandType, Sql)"/>
        Task<List<T>> FetchAsync<T>(CommandType commandType, Sql sql);

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, CommandType, string, object[])"/>
        Task<List<T>> FetchAsync<T>(CommandType commandType, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for the specified command type and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, CommandType, string, object[])"/>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, Sql sql);

        /// <summary>
        /// Executes a query for the specified command type and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, CommandType commandType, string sql, params object[] args);

        #endregion

        #region FetchAsync : Multi-POCO

        //...

        #endregion

        #region FetchAsync : Paged SkipTake

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, long, long)"/>
        Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage);

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, long, long, Sql)"/>
        Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage, Sql sql);

        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, long, long, string, object[])"/>
        Task<List<T>> FetchAsync<T>(long page, long maxItemsPerPage, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for a subset of records based on the specified parameters, and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// <para/>This method performs essentially the same operation as <see cref="SkipTakeAsync{T}(CancellationToken, long, long)"/>.
        /// Determining the number of records to skip, and how many to take, however, are calculated automatically based on the specified <paramref name="page"/> index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage);

        /// <remarks>
        /// This method performs essentially the same operation as <see cref="SkipTakeAsync{T}(CancellationToken, long, long, Sql)"/>.
        /// Determining the number of records to skip, and how many to take, however, are calculated automatically based on the specified <paramref name="page"/> index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="FetchAsync{T}(CancellationToken, long, long, string, object[])"/>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql sql);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This method performs essentially the same operation as <see cref="SkipTakeAsync{T}(CancellationToken, long, long, string, object[])"/>.
        /// Determining the number of records to skip, and how many to take, however, are calculated automatically based on the specified <paramref name="page"/> index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> FetchAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string sql, params object[] args);

        #endregion

        #region PageAsync

        /// <inheritdoc cref="PageAsync{T}(CancellationToken, long, long)"/>
        Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage);

        /// <inheritdoc cref="PageAsync{T}(CancellationToken, long, long, Sql)"/>
        Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, Sql sql);

        /// <inheritdoc cref="PageAsync{T}(CancellationToken, long, long, string, object[])"/>
        Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, string sql, params object[] args);

        /// <inheritdoc cref="PageAsync{T}(CancellationToken, long, long, Sql, Sql)"/>
        Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, Sql countSql, Sql pageSql);

        /// <inheritdoc cref="PageAsync{T}(CancellationToken, long, long, string, object[], string, object[])"/>
        Task<Page<T>> PageAsync<T>(long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for a subset of records based on the specified parameters, and returns the results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an initialized <see cref="Page{T}"/> containing a list of POCOs.</returns>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an initialized <see cref="Page{T}"/> containing a list of POCOs.</returns>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql sql);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an initialized <see cref="Page{T}"/> containing a list of POCOs.</returns>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="countSql">An SQL builder instance representing the SQL statement and its parameters, used to query the total number of records.</param>
        /// <param name="pageSql">An SQL builder instance representing the SQL statement and its parameters, used to retrieve a single page of results.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an initialized <see cref="Page{T}"/> containing a list of POCOs.</returns>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, Sql countSql, Sql pageSql);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="countSql">The SQL statement used to query the total number of records.</param>
        /// <param name="countArgs">The parameters to embed in <paramref name="countSql"/>.</param>
        /// <param name="pageSql">The SQL statement used to retrieve a single page of results.</param>
        /// <param name="pageArgs">The parameters to embed in the <paramref name="pageSql"/> string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an initialized <see cref="Page{T}"/> containing a list of POCOs.</returns>
        Task<Page<T>> PageAsync<T>(CancellationToken cancellationToken, long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs);

        #endregion

        #region SkipTakeAsync

        /// <inheritdoc cref="SkipTakeAsync{T}(CancellationToken, long, long)"/>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take);

        /// <inheritdoc cref="SkipTakeAsync{T}(CancellationToken, long, long, Sql)"/>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take, Sql sql);

        /// <inheritdoc cref="SkipTakeAsync{T}(CancellationToken, long, long, string, object[])"/>
        Task<List<T>> SkipTakeAsync<T>(long skip, long take, string sql, params object[] args);

        /// <summary>
        /// Asynchronously executes an auto-select query (<c>SELECT *</c>) for a subset of records based on the specified parameters, and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="SkipTakeAsync{T}(CancellationToken, long, long, string, object[])"/>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, Sql sql);

        /// <summary>
        /// Asynchronously executes a query for a subset of records based on the specified parameters, and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of POCOs of type <typeparamref name="T"/>.</returns>
        Task<List<T>> SkipTakeAsync<T>(CancellationToken cancellationToken, long skip, long take, string sql, params object[] args);

        #endregion

        #region ExistsAsync

        /// <inheritdoc cref="ExistsAsync{T}(CancellationToken, object)"/>
        Task<bool> ExistsAsync<T>(object pocoOrPrimaryKeyValue);

        // TODO: Missing overload: `IQueryAsync.ExistsAsync<T>(Sql)`

        /// <inheritdoc cref="ExistsAsync{T}(CancellationToken, string, object[])"/>
        Task<bool> ExistsAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously determines whether a record exists with the specified primary key value.
        /// </summary>
        /// <remarks>
        /// If provided a POCO object as the <paramref name="pocoOrPrimaryKeyValue"/> parameter, PetaPoco will extract the value from the POCO's mapped primary key property, and perform the same query as if the primary key value was provided directly.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="pocoOrPrimaryKeyValue">The primary key value, or a POCO containing an assigned primary key value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if one or more records exist with the specified primary key value; otherwise, <see langword="false"/>.</returns>
        Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, object pocoOrPrimaryKeyValue);

        // TODO: Missing overload: `IQueryAsync.ExistsAsync<T>(CancellationToken, Sql)`

        /// <summary>
        /// Asynchronously determines whether a record exists that matches the conditions defined by the specified query.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if one or more records exist that satisfy the conditions defined in the specified query; otherwise, <see langword="false"/>.</returns>
        Task<bool> ExistsAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region SingleAsync

        /// <inheritdoc cref="SingleAsync{T}(CancellationToken, object)"/>
        Task<T> SingleAsync<T>(object primaryKey);

        /// <inheritdoc cref="SingleAsync{T}(CancellationToken, Sql)"/>
        Task<T> SingleAsync<T>(Sql sql);

        /// <inheritdoc cref="SingleAsync{T}(CancellationToken, string, object[])"/>
        Task<T> SingleAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously returns the only record that matches the specified primary key value, and throws an exception if there is not exactly one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="primaryKey">The primary key value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty, or the result set contains more than one record.</exception>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, object primaryKey);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="SingleAsync{T}(CancellationToken, string, object[])"/>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously returns the only record that matches the specified query, and throws an exception if there is not exactly one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty, or the result set contains more than one record.</exception>
        Task<T> SingleAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region SingleOrDefaultAsync

        /// <inheritdoc cref="SingleOrDefaultAsync{T}(CancellationToken, object)"/>
        Task<T> SingleOrDefaultAsync<T>(object primaryKey);

        /// <inheritdoc cref="SingleOrDefaultAsync{T}(CancellationToken, Sql)"/>
        Task<T> SingleOrDefaultAsync<T>(Sql sql);

        /// <inheritdoc cref="SingleOrDefaultAsync{T}(CancellationToken, string, object[])"/>
        Task<T> SingleOrDefaultAsync<T>(string sql, params object[] args);

        /// <summary>
        /// Asynchronously returns the only record that matches the specified primary key value, or a default value if the result set is empty; this method throws an exception if there is more than one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="primaryKey">The primary key value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains default(T) if no record is found; otherwise, the single result returned by the specified query.</returns>
        /// <exception cref="InvalidOperationException">The result set contains more than one record.</exception>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, object primaryKey);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="SingleOrDefaultAsync{T}(CancellationToken, string, object[])"/>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously returns the only record that matches the specified query, or a default value if the result set is empty; this method throws an exception if there is more than one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains default(T) if no record is found; otherwise, the single result returned by the specified query.</returns>
        /// <exception cref="InvalidOperationException">The result set contains more than one record.</exception>
        Task<T> SingleOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region FirstAsync

        /// <inheritdoc cref="FirstAsync{T}(CancellationToken, Sql)"/>
        Task<T> FirstAsync<T>(Sql sql);

        /// <inheritdoc cref="FirstAsync{T}(CancellationToken, string, object[])"/>
        Task<T> FirstAsync<T>(string sql, params object[] args);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <inheritdoc cref="FirstAsync{T}(CancellationToken, string, object[])"/>
        Task<T> FirstAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously returns the first result returned by the specified query, and throws an exception if the result set is empty.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first result returned by the specified query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty.</exception>
        Task<T> FirstAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion

        #region FirstOrDefaultAsync

        /// <inheritdoc cref="FirstOrDefaultAsync{T}(CancellationToken, Sql)"/>
        Task<T> FirstOrDefaultAsync<T>(Sql sql);

        /// <inheritdoc cref="FirstOrDefaultAsync{T}(CancellationToken, string, object[])"/>
        Task<T> FirstOrDefaultAsync<T>(string sql, params object[] args);

        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="FirstOrDefaultAsync{T}(CancellationToken, string, object[])"/>
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, Sql sql);

        /// <summary>
        /// Asynchronously returns the first result returned by the specified query, or a default value if the result set is empty.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains default(T) if the result set is empty; otherwise, the first result returned by the specified query.</returns>
        Task<T> FirstOrDefaultAsync<T>(CancellationToken cancellationToken, string sql, params object[] args);

        #endregion
    }
#endif
}
