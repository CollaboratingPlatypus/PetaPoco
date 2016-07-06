// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/03/23</date>
using System;
using System.Collections.Generic;

namespace PetaPoco
{
    public interface IGridReader : IDisposable
    {
        /// <summary>
        /// Reads from a GridReader, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>An enumerable collection of result records</returns>
        IEnumerable<T> Read<T>();

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Read<T1, T2>();

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Read<T1, T2, T3>();

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The forth POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<T1> Read<T1, T2, T3, T4>();

        /// <summary>
        /// Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Read<T1, T2, TRet>(Func<T1, T2, TRet> cb);

        /// <summary>
        /// Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Read<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb);

        /// <summary>
        /// Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The forth POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        IEnumerable<TRet> Read<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb);
    }
}