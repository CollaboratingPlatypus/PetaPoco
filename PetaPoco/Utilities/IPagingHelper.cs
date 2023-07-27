namespace PetaPoco.Utilities
{
    /// <summary>
    /// Represents the contract for a paging helper.
    /// </summary>
    public interface IPagingHelper
    {
        /// <summary>
        /// Splits the given SQL statement into parts, and initializes an <see cref="SQLParts"/> instance containing the resulting substrings at <paramref name="parts"/>.
        /// </summary>
        /// <param name="sql">The SQL string to be parsed. This value must not be <see langword="null"/>.</param>
        /// <param name="parts">When this method returns, the parsed SQL statement split into it's constituent parts, if the SQL statement could be split; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the SQL statement could be parsed; otherwise, <see langword="false"/>.</returns>
        bool SplitSQL(string sql, out SQLParts parts);
    }
}
