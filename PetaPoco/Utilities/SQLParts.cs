namespace PetaPoco.Utilities
{
    /// <summary>
    /// Represents a parsed SQL statement, providing access to its constituent parts for convenient modification and rebuilding.
    /// </summary>
    public struct SQLParts
    {
        /// <summary>
        /// Gets or sets the complete SQL statement.
        /// </summary>
        public string Sql;

        /// <summary>
        /// Gets or sets the <c>COUNT</c> clause of the SQL statement, used for operations such as Exists and paged requests.
        /// </summary>
        public string SqlCount;

        /// <summary>
        /// Gets or sets the SQL statement with the <c>SELECT</c> clause removed, for generating auto-select queries.
        /// </summary>
        public string SqlSelectRemoved;

        /// <summary>
        /// Gets or sets the <c>ORDER BY</c> clause of the SQL statement, used for sorting the records.
        /// </summary>
        public string SqlOrderBy;
    }
}
