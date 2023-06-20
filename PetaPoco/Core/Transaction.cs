namespace PetaPoco
{
    /// <summary>
    /// A scoped Transaction object to facilitate maintaining transaction depth counts and proper rollbacks.
    /// </summary>
    public class Transaction : ITransaction
    {
        private IDatabase _db;

        /// <summary>
        /// Constructs an instance using the supplied <paramref name="database"/>, and begins the transaction.
        /// </summary>
        /// <param name="database">The IDatabase instance to use.</param>
        /// <seealso cref="IDatabase.BeginTransaction"/>
        public Transaction(IDatabase database)
        {
            _db = database;
            _db.BeginTransaction();
        }

        /// <inheritdoc/>
        public void Complete()
        {
            _db.CompleteTransaction();
            _db = null;
        }


        /// <summary>
        /// Closes the transaction scope, rolling back the transaction if not completed by a call to <see cref="Complete"/>.
        /// </summary>
        /// <seealso cref="IDatabase.AbortTransaction"/>
        public void Dispose()
        {
            _db?.AbortTransaction();
        }
    }
}
