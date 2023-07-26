using System;
using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for reading a result set from a database query into an enumerable collection of single or multi-POCO objects.
    /// </summary>
    public interface IGridReader : IDisposable
    {
        #region ReadSinglePoco

        /// <summary>
        /// Performs a read, returning the results as an <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a row in the result set.</typeparam>
        /// <returns>An enumerable collection of <typeparamref name="T"/> POCOs containing the result records.</returns>
        IEnumerable<T> Read<T>();

        #endregion

        #region ReadMultiPoco : auto-mapping

        /// <inheritdoc cref="Read{T1,T2,T3,T4}()"/>
        IEnumerable<T1> Read<T1, T2>();

        /// <inheritdoc cref="Read{T1,T2,T3,T4}()"/>
        IEnumerable<T1> Read<T1, T2, T3>();

        /// <summary>
        /// Performs a multi-poco read, returning the results as an <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <returns>An enumerable collection of <typeparamref name="T1"/> POCOs containing the result records.</returns>
        IEnumerable<T1> Read<T1, T2, T3, T4>();

        #endregion

        #region ReadMultiPoco : custom-mapping

        /// <inheritdoc cref="Read{T1, T2, T3, T4, TRet}(Func{T1,T2,T3,T4,TRet})"/>
        IEnumerable<TRet> Read<T1, T2, TRet>(Func<T1, T2, TRet> func);

        /// <inheritdoc cref="Read{T1, T2, T3, T4, TRet}(Func{T1,T2,T3,T4,TRet})"/>
        IEnumerable<TRet> Read<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> func);

        /// <summary>
        /// Performs a multi-poco query, returning the results as an <see cref="IEnumerable{T}"/> collection.
        /// </summary>
        /// <typeparam name="T1">The first POCO type.</typeparam>
        /// <typeparam name="T2">The second POCO type.</typeparam>
        /// <typeparam name="T3">The third POCO type.</typeparam>
        /// <typeparam name="T4">The fourth POCO type.</typeparam>
        /// <typeparam name="TRet">The result POCO type.</typeparam>
        /// <param name="func">A callback function to used to connect the POCO instances, or <see langword="null"/> to let PetaPoco automatically deduce the relationships.</param>
        /// <returns>An enumerable collection of <typeparamref name="TRet"/> POCOs containing the result records.</returns>
        IEnumerable<TRet> Read<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> func);

        #endregion
    }
}
