using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Provides access to the result collection from a paged request.
    /// </summary>
    /// <remarks>
    /// Represents a paged result set, both providing access to the items on the current page and maintaining state information about the pagination for additional queries.
    /// </remarks>
    /// <typeparam name="T">The type of POCO objects in the returned result set.</typeparam>
    public class Page<T>
    {
        /// <summary>
        /// Gets or sets the number of the current page in the result set.
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages in the full result set.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the total number of records in the full result set.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the result records on the current page.
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Gets or sets a context object, which can be used to store additional information about the result set.
        /// </summary>
        public object Context { get; set; }
    }
}
