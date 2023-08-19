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
        /// <param name="database">The database instance this GridReader is associated with.</param>
        /// <param name="command">The database query command to execute.</param>
        /// <param name="reader">The underlying data reader for reading the result sets.</param>
        /// <param name="defaultMapper">The default mapper to be used for mapping the result sets to POCOs.</param>
        internal GridReader(Database database, IDbCommand command, IDataReader reader, IMapper defaultMapper)
        {
            _db = database;
            _command = command;
            _reader = reader;
            _defaultMapper = defaultMapper;
        }

        #region Public Read methods

        /// <inheritdoc/>
        public IEnumerable<T> Read<T>()
            => SinglePocoFromIDataReader<T>(_gridIndex);

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2>()
            => MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, null);

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2, T3>()
            => MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null);

        /// <inheritdoc/>
        public IEnumerable<T1> Read<T1, T2, T3, T4>()
            => MultiPocoFromIDataReader<T1>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null);

        /// <inheritdoc/>
        public IEnumerable<TResult> Read<T1, T2, TResult>(Func<T1, T2, TResult> projector)
            => MultiPocoFromIDataReader<TResult>(_gridIndex, new Type[] { typeof(T1), typeof(T2) }, projector);

        /// <inheritdoc/>
        public IEnumerable<TResult> Read<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> projector)
            => MultiPocoFromIDataReader<TResult>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3) }, projector);

        /// <inheritdoc/>
        public IEnumerable<TResult> Read<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> projector)
            => MultiPocoFromIDataReader<TResult>(_gridIndex, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, projector);

        #endregion

        #region PocoFromIDataReader

        /// <summary>
        /// Reads data to a single POCO.
        /// </summary>
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="index">The zero-based row index to be read from the underlying <see cref="IDataReader"/>.</param>
        /// <returns>A POCO of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ObjectDisposedException">Called after the data reader has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Result records are consumed in the incorrect order, or more than once.</exception>
        private IEnumerable<T> SinglePocoFromIDataReader<T>(int index)
        {
            // TODO: Incorrect object name used when throwing ObjectDisposedException; should be `nameof(_reader)`
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
            finally // Ensure that calls to .First() etc progresses the data reader when there are multiple rows
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
        /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
        /// <param name="index">The zero-based row index to be read from the underlying <see cref="IDataReader"/>.</param>
        /// <param name="types">An array of types representing the POCO types in the returned result set.</param>
        /// <param name="transformer">A function used to connect the POCO instances as a single POCO of type <typeparamref name="T"/>, or
        /// <see langword="null"/> to let PetaPoco automatically deduce the relationships.</param>
        /// <returns>A composite POCO of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ObjectDisposedException">Called after the data reader has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Result records are consumed in the incorrect order, or more than once.</exception>
        private IEnumerable<T> MultiPocoFromIDataReader<T>(int index, Type[] types, object transformer)
        {
            // TODO: Incorrect object name used when throwing ObjectDisposedException; should be `nameof(_reader)`
            if (_reader == null)
                throw new ObjectDisposedException(GetType().FullName, "The data reader has been disposed");
            if (_consumed)
                throw new InvalidOperationException("Query results must be consumed in the correct order, and each result can only be consumed once");

            _consumed = true;

            try
            {
                var cmd = _command;
                var rdr = _reader;

                var factory = MultiPocoFactory.GetFactory<T>(types, cmd.Connection.ConnectionString, cmd.CommandText, rdr, _defaultMapper);

                // if no projector function provided by caller, figure out the split points and connect them ourself
                if (transformer == null)
                    transformer = MultiPocoFactory.GetAutoMapper(types.ToArray());

                bool bNeedTerminator = false;

                while (true)
                {
                    T poco;
                    try
                    {
                        if (!rdr.Read())
                            break;
                        poco = factory(rdr, transformer);
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
                    var poco = (T)(transformer as Delegate).DynamicInvoke(new object[types.Length]);
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
        /// Disposes the GridReader, closing and releasing the underlying <see cref="IDataReader"/>, <see cref="IDbCommand"/>, and shared
        /// <see cref="IConnection.Connection"/>.
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
