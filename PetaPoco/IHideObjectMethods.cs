using System;
using System.ComponentModel;

namespace PetaPoco
{
    /// <summary>
    /// An interface used to hide the four <see cref="object" /> instance methods from the API in Visual Studio intellisense.
    /// </summary>
    /// <remarks>
    /// <br/>Reference Project: MicroLite ORM (<see href="https://github.com/TrevorPilley/MicroLite"/>)
    /// <br/>Author: Trevor Pilley
    /// <br/>Source: <see href="https://github.com/TrevorPilley/MicroLite/blob/develop/MicroLite/IHideObjectMethods.cs" />
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMethods
    {
        /// <summary>
        /// Determines whether the given <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="other" /> is equal to this instance; otherwise, <see langword="false"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <returns>The type of the object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intelisense in Visual Studio")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intelisense in Visual Studio",
            MessageId = "GetType")]
        Type GetType();

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
