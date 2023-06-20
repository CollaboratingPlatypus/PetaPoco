namespace PetaPoco.Utilities
{
    /// <summary>
    /// Represents the contract for a paging helper.
    /// </summary>
    public interface IPagingHelper
    {
        /// <summary>
        /// Splits the given SQL query into its constituent parts.
        /// </summary>
        /// <remarks>
        /// This method is used to split a SQL query into its constituent parts for easier manipulation. The parts include the select clause, order by clause, and count clause.
        /// </remarks>
        /// <param name="sql">The SQL query to split.</param>
        /// <param name="parts">The parts of the SQL query.</param>
        /// <returns><see langword="true"/> if the SQL query could be split; otherwise, <see langword="false"/>.</returns>
        bool SplitSQL(string sql, out SQLParts parts);
    }
}
