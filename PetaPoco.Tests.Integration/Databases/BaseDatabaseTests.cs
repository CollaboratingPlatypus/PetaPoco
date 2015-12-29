// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/14</date>

using System;
using System.Configuration;
using System.Linq;
using PetaPoco.Tests.Integration.Models;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabaseTests : BaseDatabase
    {
        private Note _note = new Note
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

        [Fact]
        public void Construct_GivenConnectionStringName_ShouldBeValid()
        {
            var connectionString = DB.ConnectionString;
            var entry = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(c =>
                c.ConnectionString.Equals(connectionString));

            using (var db = new Database(entry.Name))
            {
                AfterDbCreate(db);
                var key = db.Insert(_note);
                var otherNote = db.SingleOrDefault<Note>(key);
                _note.ShouldBe(otherNote);
            }
        }

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
    }
}