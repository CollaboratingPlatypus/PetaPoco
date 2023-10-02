using System;

namespace PetaPoco
{
    /// <summary>
    /// Represents the contract for the transaction.
    /// </summary>
    /// <remarks>
    /// A PetaPoco helper to support transactions inside the using block syntax.
    /// </remarks>
    public interface ITransaction : IDisposable, IHideObjectMethods
    {
        /// <summary>
        /// Completes the transaction.
        /// </summary>
        void Complete();
    }
}
