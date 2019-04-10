using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabaseTests : BaseDatabase
    {
        private readonly Note _note = new Note
        {
            Text = "A test note",
            CreatedOn = new DateTime(1955, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        protected BaseDatabaseTests(DBTestProvider provider)
            : base(provider)
        {
        }

        protected virtual void AfterDbCreate(Database db)
        {
        }

        [Fact]
        public void Construct_GivenConnection_ShouldBeValid()
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
        public void Construct_GivenConnectionStringAndProviderName_ShouldBeValid()
        {
            var providerName = DB.Provider.GetFactory().CreateConnection().GetType().Name;
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
        public void Construct_GivenConnectionStringAndProviderFactory_ShouldBeValid()
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
        public void Construct_GivenConnectionStringName_ShouldBeValid()
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
        public void Construct_GivenConnectionStringAndProvider_ShouldBeValid()
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
        public void IsolationLevel_WhenChangedDuringTransaction_ShouldThrow()
        {
            using (DB.GetTransaction())
            {
                Should.Throw<InvalidOperationException>(() => DB.IsolationLevel = IsolationLevel.Chaos);
            }
        }

        [Fact]
        public virtual void BeginTransaction_WhenIsolationLevelIsSet_ShouldBeOfIsolationLevel()
        {
            DB.IsolationLevel = IsolationLevel.Serializable;
            using (var t = DB.GetTransaction())
            {
                var transaction = (IDbTransaction) DB.GetType().GetField("_transaction", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(DB);
                transaction.IsolationLevel.ShouldBe(DB.IsolationLevel.Value);
            }
        }

        [Fact]
        public void OpenShredConnection_WhenCalled_ShouldBeValid()
        {
            DB.Connection.ShouldBeNull();
            DB.OpenSharedConnection();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }

        [Fact]
        public async void OpenSharedConnectionAsync_WhenCalled_ShouldBeValid()
        {
            DB.Connection.ShouldBeNull();
            await DB.OpenSharedConnectionAsync();
            DB.Connection.State.ShouldBe(ConnectionState.Open);
            DB.CloseSharedConnection();
        }
    }
}