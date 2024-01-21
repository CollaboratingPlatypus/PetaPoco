using System;
using System.ComponentModel;

namespace PetaPoco
{
    /// <summary>
    /// Hides the compiler-generated public <see cref="object"/> instance methods from the list of intellisense code completion suggestions.
    /// </summary>
    /// <remarks>
    /// Reference Project: MicroLite ORM (<see href="https://github.com/TrevorPilley/MicroLite"/>)
    /// <br/>Author: Trevor Pilley
    /// <br/>Source: <see href="https://github.com/TrevorPilley/MicroLite/blob/develop/MicroLite/IHideObjectMethods.cs"/>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMethods
    {
        /// <inheritdoc cref="object.Equals(object)"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

        /// <inheritdoc cref="object.GetHashCode()"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <inheritdoc cref="object.GetType()"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intellisense")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intellisense",
            MessageId = "GetType")]
        Type GetType();

        /// <inheritdoc cref="object.ToString()"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}
