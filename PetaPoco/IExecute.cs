namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for executing SQL non-query commands and scalar queries.
    /// </summary>
    public interface IExecute
    {
        /// <summary>
        /// Executes a non-query command and returns the number of rows affected.
        /// </summary>
        /// <param name="sql">An SQL builder instance representing the SQL statement and its parameters.</param>
        /// <returns>The number of rows affected.</returns>
        int Execute(Sql sql);

        /// <summary>
        /// Executes a non-query command and returns the number of rows affected.
        /// </summary>
        /// <param name="sql">The SQL string.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>The number of rows affected.</returns>
        int Execute(string sql, params object[] args);

        /// <summary>
        /// Executes a scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="sql">An SQL builder instance representing the SQL statement and its parameters.</param>
        /// <returns>The scalar result value of type <typeparamref name="T"/>.</returns>
        T ExecuteScalar<T>(Sql sql);

        /// <summary>
        /// Executes a scalar command and returns the first column of the first row in the result set.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="args">The parameters to embed in the SQL query string.</param>
        /// <returns>The scalar result value of type <typeparamref name="T"/>.</returns>
        T ExecuteScalar<T>(string sql, params object[] args);
    }
}
