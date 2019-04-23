namespace PetaPoco
{
    public interface IExecute
    {
        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">The SQL statement to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of rows affected</returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        ///     Executes a non-query command
        /// </summary>
        /// <param name="sql">An SQL builder object representing the query and its arguments</param>
        /// <returns>The number of rows affected</returns>
        int Execute(Sql sql);

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">The SQL query to execute</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The scalar value cast to T</returns>
        T ExecuteScalar<T>(string sql, params object[] args);

        /// <summary>
        ///     Executes a query and return the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to</typeparam>
        /// <param name="sql">An SQL builder object representing the query and its arguments</param>
        /// <returns>The scalar value cast to T</returns>
        T ExecuteScalar<T>(Sql sql);
    }
}