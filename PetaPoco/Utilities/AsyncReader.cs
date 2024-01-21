using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PetaPoco.Utilities
{
#if ASYNC
    /// <summary>
    /// Represents an asynchronous reader that reads a sequence of rows from a data source.
    /// </summary>
    /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
    public class AsyncReader<T> : IAsyncReader<T>
    {
        private readonly bool _isAsync;
        private readonly Func<IDataReader, T> _pocoFactory;
        private IDatabase _db;
        private IDbCommand _cmd;
        private IDataReader _reader;
        private DbDataReader Reader => (DbDataReader)_reader;

        private AsyncReader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncReader class.
        /// </summary>
        /// <param name="database">The database instance this AsyncReader is associated with.</param>
        /// <param name="command">The database query command to execute.</param>
        /// <param name="reader">The underlying data reader for reading the result sets.</param>
        /// <param name="pocoFactory">The factory function to use for creating POCOs of type <typeparamref name="T"/>.</param>
        public AsyncReader(IDatabase database, IDbCommand command, IDataReader reader, Func<IDataReader, T> pocoFactory)
        {
            _db = database;
            _cmd = command;
            _reader = reader;
            _pocoFactory = pocoFactory;
            _isAsync = reader is DbDataReader;
        }

        /// <inheritdoc/>
        public T Poco { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> ReadAsync()
        {
            if (_reader == null)
                return false;

            Poco = default(T);

            var hasRecords = _isAsync ? await Reader.ReadAsync() : _reader.Read();

            if (hasRecords)
                Poco = _pocoFactory(_reader);

            return hasRecords;
        }

        // TODO: Not implemented: `ReadAsync(CancellationToken)`

        /// <summary>
        /// Disposes the AsyncReader, closing and releasing the underlying <see cref="IDataReader"/>, <see cref="IDbCommand"/>, and shared
        /// <see cref="IConnection.Connection"/>.
        /// </summary>
        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;

            _cmd?.Dispose();
            _cmd = null;

            _db?.CloseSharedConnection();
            _db = null;
        }

        /// <summary>
        /// Returns an empty AsyncReader.
        /// </summary>
        /// <returns>An empty, uninitialized AsyncReader of type <typeparamref name="T"/>.</returns>
        public static AsyncReader<T> Empty() => new AsyncReader<T>();
    }
#endif
}
