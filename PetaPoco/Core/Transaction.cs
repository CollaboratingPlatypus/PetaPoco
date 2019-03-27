namespace PetaPoco
{
    /// <summary>
    ///     Transaction object helps maintain transaction depth counts
    /// </summary>
    public class Transaction : ITransaction
    {
        private Database _db;

        public Transaction(Database db)
        {
            _db = db;
            _db.BeginTransaction();
        }

        public void Complete()
        {
            _db.CompleteTransaction();
            _db = null;
        }

        public void Dispose()
        {
            if (_db != null)
                _db.AbortTransaction();
        }
    }
}