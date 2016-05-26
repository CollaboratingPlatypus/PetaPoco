// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/03/16</date>
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Internal;

namespace PetaPoco
{
    public class GridReader : IGridReader
    {
        private IDataReader _reader;
        private IDbCommand _command;
        private readonly Database _db;
        private readonly IMapper _defaultMapper;

        /// <summary>
        /// The control structure for a multi-result set query
        /// </summary>
        /// <param name="database"></param>
        /// <param name="command"></param>
        /// <param name="reader"></param>
        /// <param name="defaultMapper"></param>
        internal GridReader(Database database, IDbCommand command, IDataReader reader, IMapper defaultMapper)
        {
            _db = database;
            _command = command;
            _reader = reader;
            _defaultMapper = defaultMapper;
        }

        #region public Read<T> methods

        /// <summary>
        /// Reads from a GridReader, returning the results as an IEnumerable collection
        /// </summary>
        /// <typeparam name="T">The Type representing a row in the result set</typeparam>
        /// <returns>An enumerable collection of result records</returns>
        public IEnumerable<T> Read<T>()
        {
            return SinglePocoFromIDataReader<T>(_gridIndex);
        }

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Read<T1, T2>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, null);
        }

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Read<T1, T2, T3>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null);
        }

        /// <summary>
        /// Perform a multi-poco read from a GridReader
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="T4">The forth POCO type</typeparam>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<T1> Read<T1, T2, T3, T4>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex,
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null);
        }

        /// <summary>
        /// Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Read<T1, T2, TRet>(Func<T1, T2, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, cb);
        }

        /// <summary>
        /// Perform a multi-poco query
        /// </summary>
        /// <typeparam name="T1">The first POCO type</typeparam>
        /// <typeparam name="T2">The second POCO type</typeparam>
        /// <typeparam name="T3">The third POCO type</typeparam>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        public IEnumerable<TRet> Read<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb);
        }

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
        public IEnumerable<TRet> Read<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex,
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb);
        }

        #endregion

        #region PocoFromIDataReader

        /// <summary>
        /// Read data to a single poco
        /// </summary>
        /// <typeparam name="T">The type representing a row in the result set</typeparam>
        /// <param name="index">Reader row to be read from the underlying IDataReader</param>
        /// <returns></returns>
        private IEnumerable<T> SinglePocoFromIDataReader<T>(int index)
        {
            if (_reader == null)
                throw new ObjectDisposedException(GetType().FullName, "The data reader has been disposed");
            if (_consumed)
                throw new InvalidOperationException(
                    "Query results must be consumed in the correct order, and each result can only be consumed once");
            _consumed = true;

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            try
            {
                while (index == _gridIndex)
                {
                    var factory =
                        pd.GetFactory(_command.CommandText, _command.Connection.ConnectionString, 0, _reader.FieldCount,
                            _reader, _defaultMapper) as Func<IDataReader, T>;

                    while (true)
                    {
                        T poco;
                        try
                        {
                            if (!_reader.Read())
                                yield break;
                            poco = factory(_reader);
                        }
                        catch (Exception x)
                        {
                            if (_db.OnException(x))
                                throw;
                            yield break;
                        }

                        yield return poco;
                    }
                }
            }
            finally // finally so that First etc progresses things even when multiple rows
            {
                if (index == _gridIndex)
                {
                    NextResult();
                }
            }
        }

        /// <summary>
        /// Read data to multiple pocos
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned IEnumerable</typeparam>
        /// <param name="index">Reader row to be read from the underlying IDataReader</param>
        /// <param name="types">An array of Types representing the POCO types of the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or null to automatically guess the relationships</param>
        /// <returns>A collection of POCO's as an IEnumerable</returns>
        private IEnumerable<TRet> MultiPocoFromIDataReader<TRet>(int index, Type[] types, object cb)
        {
            if (_reader == null)
                throw new ObjectDisposedException(GetType().FullName, "The data reader has been disposed");
            if (_consumed)
                throw new InvalidOperationException(
                    "Query results must be consumed in the correct order, and each result can only be consumed once");
            _consumed = true;

            try
            {
                var cmd = _command;
                var r = _reader;

                var factory = MultiPocoFactory.GetFactory<TRet>(types, cmd.Connection.ConnectionString, cmd.CommandText, r, _defaultMapper);
                if (cb == null)
                    cb = MultiPocoFactory.GetAutoMapper(types.ToArray());
                bool bNeedTerminator = false;

                while (true)
                {
                    TRet poco;
                    try
                    {
                        if (!r.Read())
                            break;
                        poco = factory(r, cb);
                    }
                    catch (Exception x)
                    {
                        if (_db.OnException(x))
                            throw;
                        yield break;
                    }

                    if (poco != null)
                        yield return poco;
                    else
                        bNeedTerminator = true;
                }
                if (bNeedTerminator)
                {
                    var poco = (TRet)(cb as Delegate).DynamicInvoke(new object[types.Length]);
                    if (poco != null)
                        yield return poco;
                    else
                        yield break;
                }
            }
            finally
            {
                if (index == _gridIndex)
                {
                    NextResult();
                }
            }
        }

        #endregion

        #region DataReader Management

        private int _gridIndex;
        private bool _consumed;

        /// <summary>
        /// Advance the IDataReader to the NextResult, if available
        /// </summary>
        private void NextResult()
        {
            if (!_reader.NextResult())
                return;
            _gridIndex++;
            _consumed = false;
        }

        /// <summary>
        /// Dispose the grid, closing and disposing both the underlying reader, command and shared connection
        /// </summary>
        public void Dispose()
        {
            if (_reader != null)
            {
                if (!_reader.IsClosed && _command != null)
                    _command.Cancel();
                _reader.Dispose();
                _reader = null;
            }

            if (_command != null)
            {
                _command.Dispose();
                _command = null;
            }
            _db.CloseSharedConnection();
        }

        #endregion
    }
}