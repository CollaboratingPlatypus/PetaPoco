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
        /// <remarks>
        /// The <c>ORDER BY</c> clause, and any row limiting clause that follows it, are removed as well, so that the value can be
        /// used as the body of a subquery.
        /// </remarks>
        public string SqlSelectRemoved;

        /// <summary>
        /// Gets or sets the <c>ORDER BY</c> clause of the SQL statement, used for sorting the records.
        /// </summary>
        /// <remarks>
        /// Contains the ordering expression list only, so that the value can be re-used inside a <c>ROW_NUMBER() OVER (...)</c>
        /// expression. The row limiting clauses that may follow the ordering expression list (<c>LIMIT</c>, <c>OFFSET ... FETCH</c>,
        /// and so on) are not part of the clause.
        /// </remarks>
        public string SqlOrderBy;

        /// <summary>
        /// Gets or sets the common table expression list that the SQL statement declares, or <see langword="null"/> when it declares
        /// none.
        /// </summary>
        /// <remarks>
        /// The <c>SELECT</c> statement that follows the list may refer to the expressions that the list declares, so a statement
        /// which is derived from <see cref="Sql"/>, such as the page query built by a provider's <c>BuildPageQuery</c>, has to keep
        /// the list in front of it. The value is ready to be prepended to a statement.
        /// </remarks>
        public string SqlCte;

        /// <summary>
        /// Gets or sets a value indicating whether the SQL statement selects distinct rows.
        /// </summary>
        /// <remarks>
        /// A statement that selects distinct rows cannot be paged by adding columns to its select list, because the <c>DISTINCT</c>
        /// modifier applies to every column of the list.
        /// </remarks>
        public bool IsDistinct;
    }
}
