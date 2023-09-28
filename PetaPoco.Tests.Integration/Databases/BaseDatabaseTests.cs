using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabaseTests : BaseDatabase
    {
        // TODO: Move to base class, combine with other test data
        #region Test Data

        private readonly Note _note = new Note
        {
            Text = "A test note",
            CreatedOn = new DateTime(1955, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        #endregion

        protected BaseDatabaseTests(DBTestProvider provider)
            : base(provider)
        {
        }

        protected virtual void AfterDbCreate(Database db)
        {
        }

        [Fact]
        public virtual void Construct_GivenConnection_ShouldBeValid()
        {
            var factory = DB.Provider.GetFactory();
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = DB.ConnectionString;
                connection.Open();

                using (var db = new Database(connection))
                {
                    AfterDbCreate(db);
                    var key = db.Insert(_note);
                    var otherNote = db.SingleOrDefault<Note>(key);

                    _note.ShouldBe(otherNote);
                }
            }
        }

        [Fact]
        public virtual void Construct_GivenConnectionStringAndProviderName_ShouldBeValid()
        {
            var providerName = ProviderName;
            var connectionString = DB.ConnectionString;

            using (var db = new Database(connectionString, providerName))
            {
                AfterDbCreate(db);
                var key = db.Insert(_note);
                var otherNote = db.SingleOrDefault<Note>(key);

                _note.ShouldBe(otherNote);
            }
        }

        [Fact]
        public virtual void Construct_GivenConnectionStringAndProviderFactory_ShouldBeValid()
        {
            var factory = DB.Provider.GetFactory();
            var connectionString = DB.ConnectionString;

            using (var db = new Database(connectionString, factory))
            {
                AfterDbCreate(db);
                var key = db.Insert(_note);
                var otherNote = db.SingleOrDefault<Note>(key);

                _note.ShouldBe(otherNote);
            }
        }

#if !NETCOREAPP
        [Fact]
        public virtual void Construct_GivenConnectionStringName_ShouldBeValid()
        {
            var connectionString = DB.ConnectionString;
            var entry = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(c => c.ConnectionString.Equals(connectionString));

            using (var db = new Database(entry.Name))
            {
                AfterDbCreate(db);
                var key = db.Insert(_note);
                var otherNote = db.SingleOrDefault<Note>(key);
                _note.ShouldBe(otherNote);
            }
        }
#endif

        [Fact]
        public virtual void Construct_GivenConnectionStringAndProvider_ShouldBeValid()
        {
            var connectionString = DB.ConnectionString;
            var provider = DB.Provider;

            using (var db = new Database(connectionString, provider))
            {
                AfterDbCreate(db);
                var key = db.Insert(_note);
                var otherNote = db.SingleOrDefault<Note>(key);

                _note.ShouldBe(otherNote);
            }
        }

        [Fact]
        [Trait("DBFeature", "Transaction")]
        [Trait("DBFeature", "IsolationLevel")]
        public virtual void IsolationLevel_WhenChangedDuringTransaction_ShouldThrow()
        {
            using (DB.GetTransaction())
            {
                Should.Throw<InvalidOperationException>(() => DB.IsolationLevel = IsolationLevel.Chaos);
            }
        }

        [Fact]
        [Trait("DBFeature", "Transaction")]
        [Trait("DBFeature", "IsolationLevel")]
        public virtual void BeginTransaction_WhenIsolationLevelIsSet_ShouldBeOfIsolationLevel()
        {
            DB.IsolationLevel = IsolationLevel.Serializable;
            using (var t = DB.GetTransaction())
            {
                var transaction = (IDbTransaction)DB.GetType().GetField("_transaction", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(DB);
                transaction.IsolationLevel.ShouldBe(DB.IsolationLevel.Value);
            }
        }

        [Fact]
        public virtual void OpenSharedConnection_WhenCalled_ShouldBeValid()
        {
            DB.Connection.ShouldBeNull();
            DB.OpenSharedConnection();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        public virtual async Task OpenSharedConnectionAsync_WhenCalled_ShouldBeValid()
        {
            DB.Connection.ShouldBeNull();
            await DB.OpenSharedConnectionAsync();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual void OpenSharedConnection_WhenCalled_ShouldInvokeOnConnectionOpening()
        {
            bool eventInvoked = false;
            // NOTE: Casting DB to Database, since `ConnectionOpening` is missing in IDatabase
            (DB as Database).ConnectionOpening += (_, e) => { eventInvoked = true; e.Connection.State.ShouldBe(ConnectionState.Closed); };

            DB.Connection.ShouldBeNull();
            DB.OpenSharedConnection();
            eventInvoked.ShouldBeTrue();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual async Task OpenSharedConnectionAsync_WhenCalled_ShouldInvokeOnConnectionOpening()
        {
            bool eventInvoked = false;
            // NOTE: Casting DB to Database, since `ConnectionOpening` is missing in IDatabase
            (DB as Database).ConnectionOpening += (_, e) => { eventInvoked = true; e.Connection.State.ShouldBe(ConnectionState.Closed); };

            DB.Connection.ShouldBeNull();
            await DB.OpenSharedConnectionAsync();
            eventInvoked.ShouldBeTrue();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual void OpenSharedConnection_AfterBeingCalled_ShouldInvokeOnConnectionOpened()
        {
            bool eventInvoked = false;
            DB.ConnectionOpened += (_, e) => { eventInvoked = true; e.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.Connection.ShouldBeNull();
            DB.OpenSharedConnection();
            eventInvoked.ShouldBeTrue();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual async Task OpenSharedConnectionAsync_AfterBeingCalled_ShouldInvokeOnConnectionOpened()
        {
            bool eventInvoked = false;
            DB.ConnectionOpened += (_, e) => { eventInvoked = true; e.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.Connection.ShouldBeNull();
            await DB.OpenSharedConnectionAsync();
            eventInvoked.ShouldBeTrue();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual void CloseSharedConnection_WhenCalled_ShouldInvokeOnConnectionClosing()
        {
            bool eventInvoked = false;
            DB.ConnectionClosing += (_, e) => { eventInvoked = true; e.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.OpenSharedConnection();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
            eventInvoked.ShouldBeTrue();
            DB.Connection.ShouldBeNull();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual void CommandHelper_WhenCalled_ShouldInvokeOnExecutingCommand()
        {
            bool eventInvoked = false;
            DB.CommandExecuting += (_, e) => { eventInvoked = true; DB.LastSQL.ShouldNotBe(e.Command.CommandText); };

            DB.OpenSharedConnection();
            DB.Insert(_note);
            DB.Exists<Note>(_note);
            eventInvoked.ShouldBeTrue();
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual async Task CommandHelperAsync_WhenCalled_ShouldInvokeOnExecutingCommand()
        {
            bool eventInvoked = false;
            DB.CommandExecuting += (_, e) => { eventInvoked = true; DB.LastSQL.ShouldNotBe(e.Command.CommandText); };

            DB.OpenSharedConnection();
            await DB.InsertAsync(_note);
            await DB.ExistsAsync<Note>(_note);
            eventInvoked.ShouldBeTrue();
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual void CommandHelper_AfterBeingCalled_ShouldInvokeOnExecutedCommand()
        {
            bool eventInvoked = false;
            DB.CommandExecuted += (_, e) => { eventInvoked = true; DB.LastSQL.ShouldBe(e.Command.CommandText); };

            DB.OpenSharedConnection();
            DB.Insert(_note);
            DB.Exists<Note>(_note);
            eventInvoked.ShouldBeTrue();
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        public virtual async Task CommandHelperAsync_AfterBeingCalled_ShouldInvokeOnExecutedCommand()
        {
            bool eventInvoked = false;
            DB.CommandExecuted += (_, e) => { eventInvoked = true; DB.LastSQL.ShouldBe(e.Command.CommandText); };

            DB.OpenSharedConnection();
            await DB.InsertAsync(_note);
            await DB.ExistsAsync<Note>(_note);
            eventInvoked.ShouldBeTrue();
            DB.CloseSharedConnection();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual void BeginTransaction_AfterBeingCalled_ShouldInvokeOnBeginTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionStarted += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.BeginTransaction();
            eventInvoked.ShouldBeTrue();
            DB.AbortTransaction();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual async Task BeginTransactionAsync_AfterBeingCalled_ShouldInvokeOnBeginTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionStarted += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            await DB.BeginTransactionAsync();
            eventInvoked.ShouldBeTrue();
            DB.AbortTransaction();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual void CompleteTransaction_WhenCalled_ShouldInvokeOnEndTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionEnding += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.BeginTransaction();
            DB.CompleteTransaction();
            eventInvoked.ShouldBeTrue();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual async Task CompleteTransactionAsync_WhenCalled_ShouldInvokeOnEndTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionEnding += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            await DB.BeginTransactionAsync();
            // NOTE: Casting DB to Database, since `CompleteTransactionAsync` is missing in IDatabase
            await (DB as Database).CompleteTransactionAsync();
            eventInvoked.ShouldBeTrue();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual void AbortTransaction_WhenCalled_ShouldInvokeOnEndTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionEnding += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            DB.BeginTransaction();
            DB.AbortTransaction();
            eventInvoked.ShouldBeTrue();
        }

        [Fact]
        [Trait("LibFeature", "Event")]
        [Trait("DBFeature", "Transaction")]
        public virtual async Task AbortTransactionAsync_WhenCalled_ShouldInvokeOnEndTransaction()
        {
            bool eventInvoked = false;
            DB.TransactionEnding += (_, e) => { eventInvoked = true; e.Transaction.Connection.State.ShouldBe(ConnectionState.Open); };

            await DB.BeginTransactionAsync();
            // NOTE: Casting DB to Database, since `AbortTransactionAsync` is missing in IDatabase
            await (DB as Database).AbortTransactionAsync();
            eventInvoked.ShouldBeTrue();
        }
    }
}
