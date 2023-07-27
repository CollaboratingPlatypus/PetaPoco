using System;
using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for reading a result set from a database query into a sequence of single or multi-POCO objects.
    /// </summary>
    public interface IGridReader : IDisposable
    {
        #region Read : Single-POCO

        /// <summary>
        /// Reads a sequence of results from a data reader.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <returns>An enumerable sequence of results of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> Read<T>();

        #endregion

        #region Read with Default Mapping : Multi-POCO

        /// <inheritdoc cref="Read{T1,T2,T3,T4}()"/>
        IEnumerable<T1> Read<T1, T2>();

        /// <inheritdoc cref="Read{T1,T2,T3,T4}()"/>
        IEnumerable<T1> Read<T1, T2, T3>();

        /// <summary>
        /// Reads a sequence of results from a data reader and projects them into a new form of type <typeparamref name="T1"/> using a default mapping function.
        /// </summary>
        /// <remarks>
        /// PetaPoco will automatically attempt to determine the split points and auto-map any additional POCO types into <typeparamref name="T1"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type, and the projected POCO type representing a single composite result record.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <returns>An enumerable sequence of results of type <typeparamref name="T1"/>.</returns>
        IEnumerable<T1> Read<T1, T2, T3, T4>();

        #endregion

        #region Read with Custom Mapping : Multi-POCO

        /// <inheritdoc cref="Read{T1, T2, T3, T4, TResult}(Func{T1, T2, T3, T4, TResult})"/>
        IEnumerable<TResult> Read<T1, T2, TResult>(Func<T1, T2, TResult> projector);

        /// <inheritdoc cref="Read{T1, T2, T3, T4, TResult}(Func{T1, T2, T3, T4, TResult})"/>
        IEnumerable<TResult> Read<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector);

        /// <summary>
        /// Reads a sequence of results from a data reader and projects them into a new form of type <typeparamref name="TResult"/> using the provided mapping function.
        /// </summary>
        /// <remarks>
        /// If <paramref name="projector"/> is <see langword="null"/>, PetaPoco will automatically attempt to determine the split points and auto-map each POCO type into <typeparamref name="TResult"/>.
        /// </remarks>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TResult">The projected POCO type representing a single result record.</typeparam>
        /// <param name="projector">A function that transforms each of the given types into a <typeparamref name="TResult"/>.</param>
        /// <returns>An enumerable sequence of results of type <typeparamref name="TResult"/>.</returns>
        IEnumerable<TResult> Read<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector);

        #endregion
    }
}
