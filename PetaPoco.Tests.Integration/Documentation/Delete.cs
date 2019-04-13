using System;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Databases;
using PetaPoco.Tests.Integration.Databases.MSSQL;
using PetaPoco.Tests.Integration.Documentation.Pocos;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Documentation
{
    [Collection("MssqlTests")]
    public class Deletes : BaseDatabase
    {
        public Deletes()
            : base(new MssqlDBTestProvider())
        {
            PocoData.FlushCaches();
        }

        [Fact]
        public void DeleteByPoco()
        {
            // Create the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };

            // Tell PetaPoco to insert it
            DB.Insert(person);
            
            // Obviously, we find only 1 matching person in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [People] WHERE [Id] = @Id", new { person.Id });
            count.ShouldBe(1);
            
            // Tell PetaPoco to delete it
            DB.Delete(person);
            
            // Obviously, we should now have none in the db
            count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [People] WHERE [Id] = @0", person.Id);
            count.ShouldBe(0);  
        }

        [Fact]
        public void DeleteByPrimaryKey()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");
            
            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Insert(note);
            
            // As note.id is auto increment, we should have an id of 1
            note.Id.ShouldBe(1);
            
            // Obviously, we should find only one matching note in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @Id", new { note.Id });
            count.ShouldBe(1);
            
            // Now, tell PetaPoco to delete a note with the id of 1
            DB.Delete<Note>(note.Id);
            
            // Obviously, we should now have none in the db
            count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @0", note.Id);
            count.ShouldBe(0);
        }
        
        [Fact]
        public void DeleteCustomWhere()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");
            
            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Insert(note);
            
            // As note.id is auto increment, we should have an id of 1
            note.Id.ShouldBe(1);
            
            // Obviously, we should find only one matching note in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @Id", new { note.Id });
            count.ShouldBe(1);
            
            // Now, we'll tell PetaPoco how to delete the note
            DB.Delete<Note>("WHERE [Id] = @Id", new { note.Id });
            
            // Obviously, we should now have none in the db
            count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @0", note.Id);
            count.ShouldBe(0);
        }
        
        [Fact]
        public void DeleteCustomSqlWhere()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");
            
            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Insert(note);
            
            // As note.id is auto increment, we should have an id of 1
            note.Id.ShouldBe(1);
            
            // Obviously, we should find only one matching note in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @Id", new { note.Id });
            count.ShouldBe(1);
            
            // Now, we'll tell PetaPoco how to delete the note
            var sql = new Sql();
            sql.Where("[Id] = @Id", new { note.Id });
            DB.Delete<Note>(sql);
            
            // Obviously, we should now have none in the db
            count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @0", note.Id);
            count.ShouldBe(0);
        }
        
        [Fact]
        public void DeleteAdvanced()
        {
            // Create the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };

            // Tell PetaPoco to insert it, but to the table SpecificPeople and not People
            DB.Insert("SpecificPeople", person);
            
            // Obviously, we find only 1 matching person in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [SpecificPeople] WHERE [Id] = @Id", new { person.Id });
            count.ShouldBe(1);
            
            // Tell PetaPoco to delete it, but in the table SpecificPeople and not People
            DB.Delete("SpecificPeople", "Id", person);
            
            // Obviously, we should now have none in the db
            count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [SpecificPeople] WHERE [Id] = @0", person.Id);
            count.ShouldBe(0);  
        }
    }
}