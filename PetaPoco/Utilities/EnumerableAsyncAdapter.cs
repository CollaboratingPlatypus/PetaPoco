using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PetaPoco.Utilities
{
#if ASYNC
    public class EnumerableAsyncAdapter<T> : IEnumerableAsyncAdapter<T>
    {
        private IDatabase _db;
        private IDbCommand _cmd;
        private IDataReader _reader;
        private readonly Func<IDataReader, T> _pocoFactory;
        private readonly bool _isAsync;
            
        private DbDataReader Reader => (DbDataReader)_reader;
        public bool HasRecords { get; private set; }

        private EnumerableAsyncAdapter()
        {
            HasRecords = false;
        }

        public EnumerableAsyncAdapter(IDatabase db, IDbCommand cmd, IDataReader reader, Func<IDataReader, T> pocoFactory)
        {
            _db = db;
            _cmd = cmd;
            _reader = reader;
            _pocoFactory = pocoFactory;
            _isAsync = reader is DbDataReader;
            HasRecords = _reader.Read();
        }

        public async Task<T> Pull()
        {
            if (!HasRecords)
                throw new InvalidOperationException("Reader contains no records.");

            if (_reader == null)
                throw new ObjectDisposedException("Async wapper is disposed.");

            var poco = _pocoFactory(_reader);
            HasRecords = _isAsync ? await Reader.ReadAsync() : _reader.Read();
            return poco;
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

        public static EnumerableAsyncAdapter<T> Empty() => new EnumerableAsyncAdapter<T>();
    }
#endif
}