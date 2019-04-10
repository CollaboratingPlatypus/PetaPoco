namespace PetaPoco.Utilities
{
    /// <summary>
    ///     Represents the contract for a paging helper.
    /// </summary>
    public interface IPagingHelper
    {
        /// <summary>
        ///     Splits the given <paramref name="sql" /> into <paramref name="parts" />;
        /// </summary>
        /// <param name="sql">The SQL to split.</param>
        /// <param name="parts">The SQL parts.</param>
        /// <returns><c>True</c> if the SQL could be split; else, <c>False</c>.</returns>
        bool SplitSQL(string sql, out SQLParts parts);
    }
}