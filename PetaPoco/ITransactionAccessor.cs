using System.Data;

namespace PetaPoco
{
    /// <summary>
    ///     Represents a contract which exposes the current <see cref="IDbTransaction" /> instance.
    /// </summary>
    public interface ITransactionAccessor
    {
        /// <summary>
        ///     Gets the current transaction instance.
        /// </summary>
        /// <returns>
        ///     The current transaction instance; else, <c>null</c> if not transaction is in progress.
        /// </returns>
        IDbTransaction Transaction { get; }
    }
}