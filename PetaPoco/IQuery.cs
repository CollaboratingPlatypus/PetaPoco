using System;
using System.Collections.Generic;

namespace PetaPoco
{
    public interface IQuery
    {
        #region Query : Single-Poco

        /// <summary>
        /// Streams the result of a select all query (SELECT *).
        /// </summary>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing of the previous one. In cases where this is an issue, consider using Fetch, which returns the results as a List rather than streaming as an IEnumerable.
        /// </remarks>
        /// <typeparam name="T">The POCO type.</typeparam>
        /// <returns>An IEnumerable collection of POCOs.</returns>
        IEnumerable<T> Query<T>();

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing of the previous one. In cases where this is an issue, consider using Fetch, which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the base SQL query and its arguments.</param>
        /// <returns>An IEnumerable collection of POCOs.</returns>
        IEnumerable<T> Query<T>(Sql sql);

        /// <summary>
        /// Runs an SQL query, returning the results as an IEnumerable collection.
        /// </summary>
        /// <remarks>
        /// For some DB providers, care should be taken to not start a new Query before finishing with and disposing of the previous one. In cases where this is an issue, consider using Fetch, which returns the results as a List rather than an IEnumerable.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>An IEnumerable collection of POCOs.</returns>
        IEnumerable<T> Query<T>(string sql, params object[] args);

        #endregion

        #region Query : Multi-Poco

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2>(Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3>(Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco query.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Performs a multi-poco query.
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable.</typeparam>
        /// <param name="types">An array of Types representing the POCO types of the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as an IEnumerable.</returns>
        IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args);

        #endregion

        #region QueryMultiple : Multi-POCO Result Set IGridReader

        /// <summary>
        /// Perform a multi-results set query.
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A GridReader to be queried.</returns>
        IGridReader QueryMultiple(Sql sql);

        /// <summary>
        /// Perform a multi-results set query.
        /// </summary>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A GridReader to be queried.</returns>
        IGridReader QueryMultiple(string sql, params object[] args);

        #endregion

        #region Fetch : Single-Poco

        /// <summary>
        /// Runs a SELECT * query and returns the result set as a typed list.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <returns>A List holding the results of the query.</returns>
        List<T> Fetch<T>();

        /// <summary>
        /// Runs a query and returns the result set as a typed list.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A List holding the results of the query.</returns>
        List<T> Fetch<T>(Sql sql);

        /// <summary>
        /// Runs a query and returns the result set as a typed list.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A List holding the results of the query.</returns>
        List<T> Fetch<T>(string sql, params object[] args);

        #endregion

        #region Fetch : Multi-Poco

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2>(Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3>(Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fourth POCO type.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, Sql sql);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args);

        /// <summary>
        /// Perform a multi-poco fetch.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TRet">The returned list POCO type.</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to automatically guess the relationships.</param>
        /// <param name="sql">The SQL query to be executed.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>A collection of POCOs as a List.</returns>
        List<TRet> Fetch<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, params object[] args);

        #endregion

        #region Fetch : Paged SkipTake

        /// <summary>
        /// Retrieves a page of records (without the total count).
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify a default SELECT * statement to only retrieve the records for the specified page.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <returns>A List of results.</returns>
        List<T> Fetch<T>(long page, long itemsPerPage);

        /// <summary>
        /// Retrieves a page of records (without the total count).
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified page.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and its arguments.</param>
        /// <returns>A List of results.</returns>
        List<T> Fetch<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        /// Retrieves a page of records (without the total count).
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified page.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sql">The base SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>A List of results.</returns>
        List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args);

        #endregion

        #region Page

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify a default SELECT * statement to only retrieve the records for the specified page.  It will also execute a second query to retrieve the total number of records in the result set.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <returns>A Page of results.</returns>
        Page<T> Page<T>(long page, long itemsPerPage);

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified page.  It will also execute a second query to retrieve the total number of records in the result set.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and its arguments.</param>
        /// <returns>A Page of results.</returns>
        Page<T> Page<T>(long page, long itemsPerPage, Sql sql);

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified page.  It will also execute a second query to retrieve the total number of records in the result set.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sql">The base SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>A Page of results.</returns>
        Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args);

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <remarks>
        /// This method allows separate SQL statements to be explicitly provided for the two parts of the page query. The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page object.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sqlCount">An SQL builder object representing the SQL to retrieve the total number of records.</param>
        /// <param name="sqlPage">An SQL builder object representing the SQL to retrieve a single page of results.</param>
        /// <returns>A Page of results.</returns>
        Page<T> Page<T>(long page, long itemsPerPage, Sql sqlCount, Sql sqlPage);

        /// <summary>
        /// Retrieves a page of records and the total number of available records.
        /// </summary>
        /// <remarks>
        /// This method allows separate SQL statements to be explicitly provided for the two parts of the page query. The page and itemsPerPage parameters are not used directly and are used simply to populate the returned Page object.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="page">The 1-based page number to retrieve.</param>
        /// <param name="itemsPerPage">The number of records per page.</param>
        /// <param name="sqlCount">The SQL to retrieve the total number of records.</param>
        /// <param name="countArgs">Arguments to any embedded parameters in the sqlCount statement.</param>
        /// <param name="sqlPage">The SQL to retrieve a single page of results.</param>
        /// <param name="pageArgs">Arguments to any embedded parameters in the sqlPage statement.</param>
        /// <returns>A Page of results.</returns>
        Page<T> Page<T>(long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs);

        #endregion

        #region SkipTake

        /// <summary>
        /// Retrieves a range of records from result set.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify a default SELECT * statement to only retrieve the records for the specified range.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over.</param>
        /// <param name="take">The number of rows to retrieve.</param>
        /// <returns>A List of results.</returns>
        List<T> SkipTake<T>(long skip, long take);

        /// <summary>
        /// Retrieves a range of records from result set.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified range.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over.</param>
        /// <param name="take">The number of rows to retrieve.</param>
        /// <param name="sql">An SQL builder object representing the base SQL query and its arguments.</param>
        /// <returns>A List of results.</returns>
        List<T> SkipTake<T>(long skip, long take, Sql sql);

        /// <summary>
        /// Retrieves a range of records from result set.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied SELECT statement to only retrieve the records for the specified range.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="skip">The number of rows at the start of the result set to skip over.</param>
        /// <param name="take">The number of rows to retrieve.</param>
        /// <param name="sql">The base SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>A List of results.</returns>
        List<T> SkipTake<T>(long skip, long take, string sql, params object[] args);

        #endregion

        #region Exists

        /// <summary>
        /// Checks for the existence of a row with the specified primary key value.
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried.</typeparam>
        /// <param name="primaryKey">The primary key value to look for.</param>
        /// <returns><see langword="true"/> if a record with the specified primary key value exists, otherwise <see langword="false"/>.</returns>
        bool Exists<T>(object primaryKey);

        /// <summary>
        /// Checks for the existence of a row matching the specified condition.
        /// </summary>
        /// <typeparam name="T">The Type representing the table being queried.</typeparam>
        /// <param name="sqlCondition">The SQL expression to be tested for (the WHERE expression).</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns><see langword="true"/> if a record matching the condition is found, otherwise <see langword="false"/>.</returns>
        bool Exists<T>(string sqlCondition, params object[] args);

        #endregion

        #region Single

        /// <summary>
        /// Returns the record with the specified primary key value.
        /// </summary>
        /// <remarks>
        /// Throws an exception if there is not exactly one record with the specified primary key value.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch.</param>
        /// <returns>The single record matching the specified primary key value.</returns>
        T Single<T>(object primaryKey);

        /// <summary>
        /// Runs a query that should always return a single row.
        /// </summary>
        /// <remarks>
        /// Throws an exception if there is not exactly one matching record
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>The single record matching the specified SQL query.</returns>
        T Single<T>(Sql sql);

        /// <summary>
        /// Runs a query that should always return a single row.
        /// </summary>
        /// <remarks>
        /// Throws an exception if there is not exactly one record
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The single record matching the specified SQL query.</returns>
        T Single<T>(string sql, params object[] args);

        #endregion

        #region SingleOrDefault

        /// <summary>
        /// Returns the record with the specified primary key value, or the default value if not found.
        /// </summary>
        /// <remarks>
        /// If there are no records with the specified primary key value, <see langword="default">default(T)</see> (typically <see langword="null"/>) is returned.
        /// </remarks>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="primaryKey">The primary key value of the record to fetch.</param>
        /// <returns>The single record matching the specified primary key value.</returns>
        T SingleOrDefault<T>(object primaryKey);

        /// <summary>
        /// Runs a query that should always return either a single row, or no rows.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows.</returns>
        T SingleOrDefault<T>(Sql sql);

        /// <summary>
        /// Runs a query that should always return either a single row, or no rows.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The single record matching the specified primary key value, or default(T) if no matching rows.</returns>
        T SingleOrDefault<T>(string sql, params object[] args);

        #endregion

        #region First

        /// <summary>
        /// Runs a query that should always return at least one record.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>The first record in the result set.</returns>
        T First<T>(Sql sql);

        /// <summary>
        /// Runs a query that should always return at least one record.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The first record in the result set.</returns>
        T First<T>(string sql, params object[] args);

        #endregion

        #region FirstOrDefault

        /// <summary>
        /// Runs a query and returns the first record, or the default value if no matching records.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments.</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows.</returns>
        T FirstOrDefault<T>(Sql sql);

        /// <summary>
        /// Runs a query and returns the first record, or the default value if no matching records.
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set.</typeparam>
        /// <param name="sql">The SQL query.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The first record in the result set, or default(T) if no matching rows.</returns>
        T FirstOrDefault<T>(string sql, params object[] args);

        #endregion
    }
}
