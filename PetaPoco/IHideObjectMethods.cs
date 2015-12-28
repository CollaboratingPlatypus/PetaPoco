// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/28</date>

using System;
using System.ComponentModel;

namespace PetaPoco
{
    /// <summary>
    ///     An interface used to hide the 4 System.Object instance methods from the API in Visual Studio intellisense.
    /// </summary>
    /// <remarks>
    ///     Reference Project: MircoLite ORM (https://github.com/TrevorPilley/MicroLite)
    ///     Author: Trevor Pilley
    ///     Source: https://github.com/TrevorPilley/MicroLite/blob/develop/MicroLite/IHideObjectMethods.cs
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMethods
    {
        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <returns>The type of the object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intelisense in Visual Studio")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType",
            Justification = "The method is defined on System.Object, this interface is just to hide it from intelisense in Visual Studio")]
        Type GetType();

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();
    }
}