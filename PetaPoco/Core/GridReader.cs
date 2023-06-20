using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Internal;

namespace PetaPoco
{
    /// <summary>
    /// Provides methods for reading a result set from a database query into an enumerable collection of single or multi-POCO objects.
    /// </summary>
    public class GridReader : IGridReader
    {
        private readonly Database _db;
        private readonly IMapper _defaultMapper;
        private IDbCommand _command;
        private IDataReader _reader;

        /// <summary>
        /// Initializes a new instance of the GridReader class with the control structure for a multi-poco query result set.
        /// </summary>
        /// <param name="database">The Database instance that this GridReader is associated with.</param>
        /// <param name="command">The command that represents the database query to be executed.</param>
        /// <param name="reader">The underlying reader that will be used to read the result sets from the database query.</param>
        /// <param name="defaultMapper">The default mapper to be used for mapping the result sets to POCOs.</param>
        internal GridReader(Database database, IDbCommand command, IDataReader reader, IMapper defaultMapper)
        {
            _db = database;
            _command = command;
            _reader = reader;
            _defaultMapper = defaultMapper;
        }

#region public Read<T> methods

        /// <inheritdoc/>
        public IEnumerable<T> Read<T>()
        {
            return SinglePocoFromIDataReader<T>(_gridIndex);
        }

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, null);
        }

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2, T3>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null);
        }

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2, T3, T4>()
        {
            return MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null);
        }

        /// <inheritdoc/>
        public IEnumerable<TRet> Read<T1, T2, TRet>(Func<T1, T2, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, cb);
        }

        /// <inheritdoc/>
        public IEnumerable<TRet> Read<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb);
        }

        /// <inheritdoc/>
        public IEnumerable<TRet> Read<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb)
        {
            return MultiPocoFromIDataReader<TRet>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb);
        }

#endregion

#region PocoFromIDataReader

        /// <summary>
        /// Reads data to a single POCO.
        /// </summary>
        /// <typeparam name="T">The type representing a row in the result set.</typeparam>
        /// <param name="index">The row to be read from the underlying <see cref="IDataReader"/>.</param>
        /// <returns>A <typeparamref name="T"/> POCO.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="InvalidOperationException"/>
        private IEnumerable<T> SinglePocoFromIDataReader<T>(int index)
        {
            if (_reader == null)
                throw new ObjectDisposedException(GetType().FullName, "The data reader has been disposed");
            if (_consumed)
                throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
            _consumed = true;

            var pd = PocoData.ForType(typeof(T), _defaultMapper);
            try
            {
                while (index == _gridIndex)
                {
                    var factory = pd.GetFactory(_command.CommandText, _command.Connection.ConnectionString, 0, _reader.FieldCount, _reader, _defaultMapper) as Func<IDataReader, T>;

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
        /// Reads data to multiple POCOs.
        /// </summary>
        /// <typeparam name="TRet">The type of objects in the returned <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="index">The row to be read from the underlying <see cref="IDataReader"/>.</param>
        /// <param name="types">An array of types representing the POCO types in the returned result set.</param>
        /// <param name="cb">A callback function to connect the POCO instances, or <see langword="null"/> to let PetaPoco automatically deduce the relationships.</param>
        /// <returns>A collection of <typeparamref name="TRet"/> POCOs as an IEnumerable.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="InvalidOperationException"/>
        private IEnumerable<TRet> MultiPocoFromIDataReader<TRet>(int index, Type[] types, object cb)
        {
            if (_reader == null)
                throw new ObjectDisposedException(GetType().FullName, "The data reader has been disposed");
            if (_consumed)
                throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");
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
                    var poco = (TRet) (cb as Delegate).DynamicInvoke(new object[types.Length]);
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
        /// Advances the <see cref="IDataReader"/> to the <see cref="IDataReader.NextResult"/>, if one exists.
        /// </summary>
        private void NextResult()
        {
            if (!_reader.NextResult())
                return;
            _gridIndex++;
            _consumed = false;
        }

        /// <summary>
        /// Disposes the GridReader, closing and disposing the underlying reader, command, and shared connection.
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
