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
    /// <typeparam name="T">The type of POCO object to read from the data source.</typeparam>
    public class AsyncReader<T> : IAsyncReader<T>
    {
        private readonly bool _isAsync;
        private readonly Func<IDataReader, T> _pocoFactory;
        private IDbCommand _cmd;
        private IDatabase _db;
        private IDataReader _reader;
        private DbDataReader Reader => (DbDataReader)_reader;

        private AsyncReader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncReader class.
        /// </summary>
        /// <param name="db">The database from which to read data.</param>
        /// <param name="cmd">The command to execute against the database.</param>
        /// <param name="reader">The data reader to use for reading data.</param>
        /// <param name="pocoFactory">The factory method to be used for when creating POCOs of type <typeparamref name="T"/>.</param>
        public AsyncReader(IDatabase db, IDbCommand cmd, IDataReader reader, Func<IDataReader, T> pocoFactory)
        {
            _db = db;
            _cmd = cmd;
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

        /// <summary>
        /// Releases all resources used by the current instance.
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
