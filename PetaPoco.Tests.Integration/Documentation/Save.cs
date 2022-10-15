using System;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Databases;
using PetaPoco.Tests.Integration.Databases.MSSQL;
using PetaPoco.Tests.Integration.Documentation.Pocos;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Documentation
{
    [Collection("Mssql")]
    public class Save : BaseDatabase
    {
        public Save()
            : base(new MssqlDBTestProvider())
        {
            PocoData.FlushCaches();
        }

        [Fact]
        public void Save_Insert()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");

            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Save(note);

            // As note.id is auto increment, we should have an id of 1
            note.Id.ShouldBe(1);

            // Obviously, we should find only one matching note in the db
            var count = DB.ExecuteScalar<int>("SELECT COUNT([Id]) FROM [Note] WHERE [Id] = @Id", new { note.Id });
            count.ShouldBe(1);

            // Fetch a new copy of note
            var noteFromDb = DB.Single<Note>(note.Id);

            // They are the same
            note.Id.ShouldBe(noteFromDb.Id);
            note.Text.ShouldBe(noteFromDb.Text);
            note.CreatedOn.Ticks.ShouldBe(noteFromDb.CreatedOn.Ticks);
        }

        [Fact]
        public void Save_Update()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");

            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Save(note);

            // Update the note
            note.Text += " and this is my update";
            DB.Save(note);

            // Fetch a new copy of note
            var noteFromDb = DB.Single<Note>(note.Id);

            // The note text is the same
            note.Text.ShouldBe(noteFromDb.Text);
            note.Text.ShouldContain(" and this is my update");
        }
    }
}