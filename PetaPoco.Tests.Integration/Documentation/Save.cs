using System;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Databases;
using PetaPoco.Tests.Integration.Databases.MSSQL;
using PetaPoco.Tests.Integration.Documentation.Pocos;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Documentation
{
    [Collection("Documentation")]
    public class SaveTests : BaseDbContext
    {
        public SaveTests()
            : base(new MssqlDBTestProvider())
        {
            PocoData.FlushCaches();
        }

        [Fact]
        public void Save_Insert()
        {
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

            // The values in noteFromDb's column-mapped properties should be equal to the original poco's
            note.Id.ShouldBe(noteFromDb.Id);
            note.Text.ShouldBe(noteFromDb.Text);
            note.CreatedOn.Ticks.ShouldBe(noteFromDb.CreatedOn.Ticks);
        }

        [Fact]
        public void Save_Update()
        {
            // Add a note
            var note = new Note { Text = "This is my note", CreatedOn = DateTime.UtcNow };
            DB.Save(note);

            // Update the note
            note.Text += " and this is my update";
            DB.Save(note);

            // Fetch a new copy of note
            var noteFromDb = DB.Single<Note>(note.Id);

            // The values in noteFromDb's column-mapped properties should be equal to the original poco's
            note.Text.ShouldBe(noteFromDb.Text);
            note.Text.ShouldContain(" and this is my update");
        }
    }
}
