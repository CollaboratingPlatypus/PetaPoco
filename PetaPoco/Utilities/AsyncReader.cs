using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PetaPoco.Utilities
{
#if ASYNC
    public class AsyncReader<T> : IAsyncReader<T>
    {
        private IDatabase _db;
        private IDbCommand _cmd;
        private IDataReader _reader;
        private readonly Func<IDataReader, T> _pocoFactory;
        private readonly bool _isAsync;
        private DbDataReader Reader => (DbDataReader)_reader;
        
        public T Poco { get; private set; }

        public async Task<Boolean> ReadAsync()
        {
            if (_reader == null)
                return false;

            Poco = default(T);
            
            var hasRecords = _isAsync ? await Reader.ReadAsync() : _reader.Read();

            if (hasRecords)
                Poco = _pocoFactory(_reader);
            
            return hasRecords;
        }
        

        private AsyncReader()
        {
        }

        public AsyncReader(IDatabase db, IDbCommand cmd, IDataReader reader, Func<IDataReader, T> pocoFactory)
        {
            _db = db;
            _cmd = cmd;
            _reader = reader;
            _pocoFactory = pocoFactory;
            _isAsync = reader is DbDataReader;
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;

            _cmd?.Dispose();
            _cmd = null;

            _db?.CloseSharedConnection();
            _db = null;
        }

        public static AsyncReader<T> Empty() => new AsyncReader<T>();
    }
#endif
}