using System;
using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for executing SQL queries and returning the result set as lists, enumerables, single POCOs, multi-POCOs, or paged results.
    /// </summary>
    public interface IQuery
    {
        #region Query : Single-POCO

        /// <summary>
        /// Executes an auto-select query (<c>SELECT *</c>) and returns the results as a sequence of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// <para/><inheritdoc cref="Query{T}(Sql)"/>
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> Query<T>();

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="Query{T}(string, object[])"/>
        IEnumerable<T> Query<T>(Sql sql);

        /// <summary>
        /// Executes a query and returns the results as a sequence of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Because this method streams the results from the database, care should be taken to not start a new query before finishing with
        /// and disposing of the previous one to prevent encountering database locks. In cases where contention could be an issue, consider
        /// using the equivalent Fetch method.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> Query<T>(string sql, params object[] args);

        #endregion

        #region Query with Default Mapping : Multi-POCO

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(Sql)"/>
        IEnumerable<T1> Query<T1, T2>(Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(Sql)"/>
        IEnumerable<T1> Query<T1, T2, T3>(Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(Sql)"/>
        IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="T1"/> using a default
        /// mapping function.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically attempt to determine the split points and auto-map each additional POCO type into <typeparamref
        /// name="T1"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="T1"/>.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(string, object[])"/>
        IEnumerable<T1> Query<T1, T2>(string sql, params object[] args);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(string, object[])"/>
        IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5}(string, object[])"/>
        IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="T1"/> using a default
        /// mapping function.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically attempt to determine the split points and auto-map each additional POCO type into <typeparamref
        /// name="T1"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="T1"/>.</returns>
        IEnumerable<T1> Query<T1, T2, T3, T4, T5>(string sql, params object[] args);

        #endregion

        #region Query with Custom Mapping : Multi-POCO

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        IEnumerable<TResult> Query<T1, T2, TResult>(Func<T1, T2, TResult> projector, Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        IEnumerable<TResult> Query<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        IEnumerable<TResult> Query<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, Sql sql);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="TResult"/> using the
        /// provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and
        /// auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>, or <see
        /// langword="null"/> to use a default mapping function.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="TResult"/>.</returns>
        IEnumerable<TResult> Query<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, Sql sql);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        IEnumerable<TResult> Query<T1, T2, TResult>(Func<T1, T2, TResult> projector, string sql, params object[] args);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        IEnumerable<TResult> Query<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, string sql, params object[] args);

        /// <inheritdoc cref="Query{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        IEnumerable<TResult> Query<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, string sql, params object[] args);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="TResult"/> using the
        /// provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and
        /// auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>, or <see
        /// langword="null"/> to use a default mapping function.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="TResult"/>.</returns>
        IEnumerable<TResult> Query<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, string sql, params object[] args);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="TResult"/> using the
        /// provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and
        /// auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="types">An array of POCO types representing the types referenced in composite type <typeparamref
        /// name="TResult"/>.</param>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>, or <see
        /// langword="null"/> to use a default mapping function.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An enumerable sequence of POCOs of type <typeparamref name="TResult"/>.</returns>
        IEnumerable<TResult> Query<TResult>(Type[] types, object projector, string sql, params object[] args);

        #endregion

        #region QueryMultiple using IGridReader : Multi-POCO Result Set

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="QueryMultiple(string, object[])"/>
        IGridReader QueryMultiple(Sql sql);

        /// <summary>
        /// Executes a multi-result set query.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A GridReader for reading the sequence of results.</returns>
        IGridReader QueryMultiple(string sql, params object[] args);

        #endregion

        #region Fetch : Single-POCO

        /// <summary>
        /// Executes an auto-select query (<c>SELECT *</c>) and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        List<T> Fetch<T>();

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="Fetch{T}(string, object[])"/>
        List<T> Fetch<T>(Sql sql);

        /// <summary>
        /// Executes a query and returns the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        List<T> Fetch<T>(string sql, params object[] args);

        #endregion

        #region Fetch with Default Mapping : Multi-POCO

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(Sql)"/>
        List<T1> Fetch<T1, T2>(Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(Sql)"/>
        List<T1> Fetch<T1, T2, T3>(Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(Sql)"/>
        List<T1> Fetch<T1, T2, T3, T4>(Sql sql);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="T1"/> using a default
        /// mapping function.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically attempt to determine the split points and auto-map each additional POCO type into <typeparamref
        /// name="T1"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T1"/>.</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(string, object[])"/>
        List<T1> Fetch<T1, T2>(string sql, params object[] args);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(string, object[])"/>
        List<T1> Fetch<T1, T2, T3>(string sql, params object[] args);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5}(string, object[])"/>
        List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="T1"/> using a default
        /// mapping function.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically attempt to determine the split points and auto-map each additional POCO type into <typeparamref
        /// name="T1"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T1"/>.</returns>
        List<T1> Fetch<T1, T2, T3, T4, T5>(string sql, params object[] args);

        #endregion

        #region Fetch with Custom Mapping : Multi-POCO

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        List<TResult> Fetch<T1, T2, TResult>(Func<T1, T2, TResult> projector, Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        List<TResult> Fetch<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, Sql)"/>
        List<TResult> Fetch<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, Sql sql);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="TResult"/> using the
        /// provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and
        /// auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>, or <see
        /// langword="null"/> to use a default mapping function.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T1"/>.</returns>
        List<TResult> Fetch<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, Sql sql);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        List<TResult> Fetch<T1, T2, TResult>(Func<T1, T2, TResult> projector, string sql, params object[] args);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        List<TResult> Fetch<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector, string sql, params object[] args);

        /// <inheritdoc cref="Fetch{T1, T2, T3, T4, T5, TResult}(Func{T1, T2, T3, T4, T5, TResult}, string, object[])"/>
        List<TResult> Fetch<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector, string sql, params object[] args);

        /// <summary>
        /// Executes a multi-poco query and projects the result sequence into a new form of type <typeparamref name="TResult"/> using the
        /// provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and
        /// auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="T5">The fifth POCO type.</typeparam>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>, or <see
        /// langword="null"/> to use a default mapping function.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T1"/>.</returns>
        List<TResult> Fetch<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> projector, string sql, params object[] args);

        #endregion

        #region Fetch : Paged SkipTake

        /// <summary>
        /// Executes an auto-select query (<c>SELECT *</c>) for a subset of records based on the specified parameters, and returns the
        /// results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// <para/>This method performs essentially the same operation as <see cref="SkipTake{T}(long, long)"/>. Determining the number of
        /// records to skip, and how many to take, however, are calculated automatically based on the specified <paramref name="page"/>
        /// index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <inheritdoc cref="Fetch{T}(long, long, string, object[])"/>
        List<T> Fetch<T>(long page, long maxItemsPerPage);

        /// <remarks>
        /// This method performs essentially the same operation as <see cref="SkipTake{T}(long, long, Sql)"/>. Determining the number of
        /// records to skip, and how many to take, however, are calculated automatically based on the specified <paramref name="page"/>
        /// index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="Fetch{T}(long, long, string, object[])"/>
        List<T> Fetch<T>(long page, long maxItemsPerPage, Sql sql);

        /// <summary>
        /// Executes a query for a subset of records based on the specified parameters, and returns the results as a list of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This method performs essentially the same operation as <see cref="SkipTake{T}(long, long, string, object[])"/>. Determining the
        /// number of records to skip, and how many to take, however, are calculated automatically based on the specified <paramref
        /// name="page"/> index and <paramref name="maxItemsPerPage"/> values.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list containing at most <paramref name="maxItemsPerPage"/> POCOs of type <typeparamref name="T"/>.</returns>
        List<T> Fetch<T>(long page, long maxItemsPerPage, string sql, params object[] args);

        #endregion

        #region Page

        /// <summary>
        /// Executes an auto-select query (<c>SELECT *</c>) for a subset of records based on the specified parameters, and returns the
        /// results as a Page of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// <para/>PetaPoco will automatically modify a default <c>SELECT *</c> statement to only retrieve the records for the specified
        /// page. It will also execute a second query to retrieve the total number of records in the result set.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <returns>An initialized <see cref="PetaPoco.Page{T}"/> containing a list of POCOs.</returns>
        Page<T> Page<T>(long page, long maxItemsPerPage);

        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="Page{T}(long, long, string, object[])"/>
        Page<T> Page<T>(long page, long maxItemsPerPage, Sql sql);

        /// <summary>
        /// Executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically modify the supplied <c>SELECT</c> statement to only retrieve the records for the specified page. It
        /// will also execute a second query to retrieve the total number of records in the result set.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="page">The one-based page number used to calculate the number of records to skip.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>An initialized <see cref="PetaPoco.Page{T}"/> containing a list of POCOs.</returns>
        Page<T> Page<T>(long page, long maxItemsPerPage, string sql, params object[] args);

        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="countSql">An SQL builder instance representing the SQL statement and its parameters, used to query the total number
        /// of records.</param>
        /// <param name="pageSql">An SQL builder instance representing the SQL statement and its parameters, used to retrieve a single page
        /// of results.</param>
        /// <inheritdoc cref="Page{T}(long, long, string, object[], string, object[])"/>
        Page<T> Page<T>(long page, long maxItemsPerPage, Sql countSql, Sql pageSql);

        /// <summary>
        /// Executes a query for a subset of records based on the specified parameters, and returns the results as a Page of type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This method accepts two separate SQL statements that will be used explicitly for both parts of the page query. The <paramref
        /// name="page"/> and <paramref name="maxItemsPerPage"/> parameters are used to populate the returned Page object.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="page">The one-based page number for this page.</param>
        /// <param name="maxItemsPerPage">The maximum number of records per page.</param>
        /// <param name="countSql">The SQL statement used to query the total number of records.</param>
        /// <param name="countArgs">The parameters to embed in <paramref name="countSql"/>.</param>
        /// <param name="pageSql">The SQL statement used to retrieve a single page of results.</param>
        /// <param name="pageArgs">The parameters to embed in the <paramref name="pageSql"/> string.</param>
        /// <returns>An initialized <see cref="PetaPoco.Page{T}"/> containing a list of POCOs.</returns>
        Page<T> Page<T>(long page, long maxItemsPerPage, string countSql, object[] countArgs, string pageSql, object[] pageArgs);

        #endregion

        #region SkipTake

        /// <summary>
        /// Executes an auto-select query (<c>SELECT *</c>) and returns a subset of the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="IDatabase.EnableAutoSelect"/> must be enabled in order to generate the auto-select portion of the SQL statement.
        /// <para/><inheritdoc cref="SkipTake{T}(long, long, Sql)"/>
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        List<T> SkipTake<T>(long skip, long take);

        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="SkipTake{T}(long, long, string, object[])"/>
        List<T> SkipTake<T>(long skip, long take, Sql sql);

        /// <summary>
        /// Executes a query and returns a subset of the results as a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The provided SQL query will be modified to limit the starting offset and number of returned records based on the specified
        /// parameters.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>A list of POCOs of type <typeparamref name="T"/>.</returns>
        List<T> SkipTake<T>(long skip, long take, string sql, params object[] args);

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether a record exists with the specified primary key value.
        /// </summary>
        /// <remarks>
        /// If provided a POCO instance as the <paramref name="pocoOrPrimaryKeyValue"/> parameter, PetaPoco will extract the value from the
        /// POCO's mapped primary key property, and perform the same query as if the primary key value was provided directly.
        /// </remarks>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="pocoOrPrimaryKeyValue">The primary key value, or a POCO containing an assigned primary key value.</param>
        /// <returns><see langword="true"/> if one or more records exist with the specified primary key value; otherwise, <see
        /// langword="false"/>.</returns>
        bool Exists<T>(object pocoOrPrimaryKeyValue);

        // TODO: Missing overload: `bool IQuery.Exists<T>(Sql)`

        /// <summary>
        /// Determines whether a record exists that matches the conditions defined by the specified query.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns><see langword="true"/> if one or more records exist that satisfy the conditions defined in the specified query;
        /// otherwise, <see langword="false"/>.</returns>
        bool Exists<T>(string sql, params object[] args);

        #endregion

        #region Single

        /// <summary>
        /// Returns the only record that matches the specified primary key value, and throws an exception if there is not exactly one
        /// matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="primaryKey">The primary key value.</param>
        /// <returns>The single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty, or the result set contains more than one
        /// record.</exception>
        T Single<T>(object primaryKey);

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="Single{T}(string, object[])"/>
        T Single<T>(Sql sql);

        /// <summary>
        /// Returns the only record that matches the specified query, and throws an exception if there is not exactly one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>The single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty, or the result set contains more than one
        /// record.</exception>
        T Single<T>(string sql, params object[] args);

        #endregion

        #region SingleOrDefault

        /// <summary>
        /// Returns the only record that matches the specified primary key value, or a default value if the result set is empty; this method
        /// throws an exception if there is more than one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="primaryKey">The primary key value.</param>
        /// <returns>default(T) if no record is found; otherwise, the single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set contains more than one record.</exception>
        T SingleOrDefault<T>(object primaryKey);

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="SingleOrDefault{T}(string, object[])"/>
        T SingleOrDefault<T>(Sql sql);

        /// <summary>
        /// Returns the only record that matches the specified query, or a default value if the result set is empty; this method throws an
        /// exception if there is more than one matching record.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>default(T) if no record is found; otherwise, the single result returned by the query.</returns>
        /// <exception cref="InvalidOperationException">The result set contains more than one record.</exception>
        T SingleOrDefault<T>(string sql, params object[] args);

        #endregion

        #region First

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="First{T}(string, object[])"/>
        T First<T>(Sql sql);

        /// <summary>
        /// Returns the first record that matches the specified query, and throws an exception if the result set is empty.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>The first result record returned by the specified query.</returns>
        /// <exception cref="InvalidOperationException">The result set is empty.</exception>
        T First<T>(string sql, params object[] args);

        #endregion

        #region FirstOrDefault

        /// <param name="sql">An SQL builder instance representing the SQL query and its parameters.</param>
        /// <inheritdoc cref="FirstOrDefault{T}(string, object[])"/>
        T FirstOrDefault<T>(Sql sql);

        /// <summary>
        /// Returns the first record that matches the specified query, or a default value if the result set is empty.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>default(T) if the result set is empty; otherwise, the first record that matches the specified query.</returns>
        T FirstOrDefault<T>(string sql, params object[] args);

        #endregion
    }
}
