namespace PetaPoco
{
    /// <summary>
    /// Defines an interface for executing SQL commands and queries.
    /// </summary>
    /// <remarks>
    /// This interface provides methods for executing SQL non-query commands and scalar queries. It supports SQL commands and queries represented as strings or as <see cref="Sql">Sql Builder</see> objects.
    /// </remarks>
    public interface IExecute
    {
        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The number of rows affected.</returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sql">An Sql builder object representing the SQL statement and its arguments.</param>
        /// <returns>The number of rows affected.</returns>
        int Execute(Sql sql);

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL statement.</param>
        /// <returns>The scalar value cast to <typeparamref name="T"/>.</returns>
        T ExecuteScalar<T>(string sql, params object[] args);

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type that the result value should be cast to.</typeparam>
        /// <param name="sql">An Sql builder object representing the SQL query and its arguments.</param>
        /// <returns>The scalar value cast to <typeparamref name="T"/>.</returns>
        T ExecuteScalar<T>(Sql sql);
    }
}
