using System;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Databases;
using PetaPoco.Tests.Integration.Databases.MSSQL;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Documentation
{
    [Collection("MssqlTests")]
    public class Updates : BaseDatabase
    {
        public Updates()
            : base(new MssqlDBTestProvider())
        {
            PocoData.FlushCaches();
        }

        [Fact]
        public void Update()
        {
            // Create and insert the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };
            var id = DB.Insert(person);

            // Update a few properties of the person
            person.Age = 70;
            person.Name = "The PetaPoco";

            // Tell PetaPoco to update the DB
            DB.Update(person);

            // Get a clone/copy from the DB
            var clone = DB.Single<Person>(id);

            // See, the person has been updated
            clone.Id.ShouldBe(person.Id);
            clone.Dob.ShouldBe(person.Dob);
            clone.Height.ShouldBe(person.Height);
            clone.Age.ShouldBe(person.Age);
            clone.Name.ShouldBe(person.Name);
        }

        [Fact]
        public void UpdatePartial()
        {
            // Create and insert the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };
            var id = DB.Insert(person);

            // Update a few properties of the person
            person.Age = 70;
            person.Name = "The PetaPoco";

            // Get the poco data
            var pocoData = PocoData.ForType(person.GetType(), DB.DefaultMapper);

            // Tell PetaPoco to update only ther person's name
            // The update statement produced is `UPDATE [People] SET [FullName] = @0 WHERE [Id] = @1`
            DB.Update(person, new[] { pocoData.GetColumnName(nameof(Person.Name)) });

            // Get a clone/copy from the DB
            var clone = DB.Single<Person>(id);

            // See, the person has been updated, but only the name
            clone.Id.ShouldBe(person.Id);
            clone.Dob.ShouldBe(person.Dob);
            clone.Height.ShouldBe(person.Height);

            clone.Age.ShouldNotBe(70);
            clone.Name.ShouldBe("The PetaPoco");
        }

        [Fact]
        public void UpdateToDifferentTable()
        {
            // Create the and insert the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };
            var id = DB.Insert("SpecificPeople", "Id", person);

            // Update a few properties of the person
            person.Age = 70;
            person.Name = "The PetaPoco";

            // Tell PetaPoco to update the DB table SpecificPeople
            // The update statement produced is `UPDATE [SpecificPeople] SET [FullName] = @0, [Age] = @1, [Height] = @2, [Dob] = @3 WHERE [Id] = @4`
            DB.Update("SpecificPeople", "Id", person);

            // We need to get the clone/copy from the correct table
            // Note: we can't use auto select builder here because PetaPoco would create columns such as People.Id
            var clone = DB.Query<Person>("SELECT * FROM [SpecificPeople] sp WHERE sp.[Id] = @0", id).Single();

            // See, the person has been updated
            clone.Id.ShouldBe(person.Id);
            clone.Dob.ShouldBe(person.Dob);
            clone.Height.ShouldBe(person.Height);
            clone.Age.ShouldBe(person.Age);
            clone.Name.ShouldBe(person.Name);
        }

        [Fact]
        public void UpdateConventionalPoco()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");

            // Insert some notes using all APIs
            var note1 = new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) };
            var note2 = new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) };
            var note3 = new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) };
            var note4 = new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) };
            var note5 = new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) };

            // Each of the API usuages here are effectively the same, as PetaPoco is providing the correct unknown values. 
            // This is because the poco has been mapped by convention and therefore PetaPoco understands how to do this.
            DB.Insert(note1);
            DB.Insert(note2);
            DB.Insert(note3);
            DB.Insert(note4);
            DB.Insert(note5);

            //Update the notes
            note1.Text += " some more text";
            note2.Text += " some more text";
            note3.Text += " some more text";
            note4.Text += " some more text";
            note5.Text += " some more text";

            // Get the poco data
            var pocoData = PocoData.ForType(typeof(Note), DB.DefaultMapper);

            // Update all notes using all APIs
            DB.Update(note1);
            DB.Update(note2, note2.Id);
            DB.Update(note3, note3.Id, pocoData.UpdateColumns);
            var sql1 = $"SET {DB.Provider.EscapeSqlIdentifier(pocoData.GetColumnName(nameof(Note.Text)))} = @1 " +
                       $"WHERE {DB.Provider.EscapeSqlIdentifier(pocoData.TableInfo.PrimaryKey)} = @0";
            DB.Update<Note>(sql1, note4.Id, note4.Text);
            var sql2 = new Sql(
                $"SET {DB.Provider.EscapeSqlIdentifier(pocoData.GetColumnName(nameof(Note.Text)))} = @1 " +
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pocoData.TableInfo.PrimaryKey)} = @0", note5.Id, note5.Text);
            DB.Update<Note>(sql2);

            // Just to be sure
            DB.ExecuteScalar<int>("SELECT COUNT(*) FROM [Note] WHERE CAST(Text AS NVARCHAR(MAX)) = @0", "PetaPoco's note some more text").ShouldBe(5);
        }

        [Fact]
        public void UpdateUnconventionalPoco()
        {
            // Create the UnconventionalPocos table
            DB.Execute(@"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_SCHEMA = 'dbo' AND t.TABLE_NAME = 'TBL_UnconventionalPocos')
	                         DROP TABLE dbo.[TBL_UnconventionalPocos]

                         CREATE TABLE dbo.[TBL_UnconventionalPocos] (
	                         [PrimaryKey] INT IDENTITY(1,1) PRIMARY KEY,
	                         [Text] NTEXT NOT NULL
                         )");

            // This POCO is unconventional because, when using the default conventional mapper, PetaPoco won't understand how this poco maps to the database.
            // To understand the power of unconventional mapping, a developer could configure it to work in this situation. 
            var poco = new UnconventionalPoco { Text = "PetaPoco" };

            // Insert the poco
            var id = DB.Insert("TBL_UnconventionalPocos", "PrimaryKey", true, poco);

            // Update the poco
            poco.Text += " some more text";
            DB.Update("TBL_UnconventionalPocos", "PrimaryKey", poco);

            // Get a clone/copy from the DB
            var clone = DB.Query<UnconventionalPoco>("SELECT * FROM [TBL_UnconventionalPocos] WHERE [PrimaryKey] = @0", id).Single();

            // Just to be sure
            poco.PrimaryKey.ShouldBe(clone.PrimaryKey);
            poco.Text.ShouldBe(clone.Text);
        }

        [Fact]
        public void UpdateConventionalUnconventionalPoco()
        {
            // Create the UnconventionalPocos table
            DB.Execute(@"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_SCHEMA = 'dbo' AND t.TABLE_NAME = 'TBL_UnconventionalPocos')
	                         DROP TABLE dbo.[TBL_UnconventionalPocos]

                         CREATE TABLE dbo.[TBL_UnconventionalPocos] (
                             [PrimaryKey] INT IDENTITY(1,1) PRIMARY KEY,
	                         [Text] NTEXT NOT NULL
                         )");

            // Reconfigure the convention mapper
            // Note: I can't think of a valid reason, other than for a purpose such as this, where you would configure the convention mapper in this way.
            ((ConventionMapper) DB.DefaultMapper).MapPrimaryKey = (ti, t) =>
            {
                var prop = t.GetProperties().FirstOrDefault(p => p.Name == "PrimaryKey");

                if (prop == null)
                    return false;

                ti.PrimaryKey = prop.Name;
                ti.AutoIncrement = ((ConventionMapper) DB.DefaultMapper).IsPrimaryKeyAutoIncrement(prop.PropertyType);
                return true;
            };
            ((ConventionMapper) DB.DefaultMapper).InflectTableName = (i, tn) => "TBL_" + tn + "s";

            // Create the POCO
            var poco = new UnconventionalPoco { Text = "PetaPoco" };

            // Tell PetaPoco to insert it
            var id = DB.Insert(poco);

            // Get a clone/copy from the DB
            var clone = DB.SingleOrDefault<UnconventionalPoco>(id);

            // See, they're are the same
            clone.ShouldBe(poco);

            // Update the original poco
            poco.Text += " some more text";

            // Update the poco
            DB.Update(poco);

            // Get the clone from teh database again
            clone = DB.SingleOrDefault<UnconventionalPoco>(id);

            // Confirm the text was updated
            clone.Text.ShouldBe("PetaPoco some more text");
        }

        [Fact]
        public void UpdateAnonymousPocoWithConventionalNaming()
        {
            // Create the table for our unknown but conventional POCO
            DB.Execute(@"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_SCHEMA = 'dbo' AND t.TABLE_NAME = 'XFiles')
	                         DROP TABLE dbo.[XFiles]

                         CREATE TABLE dbo.[XFiles] (
                             [Id] INT IDENTITY(1,1) PRIMARY KEY,
	                         [FileName] VARCHAR(255) NOT NULL
                         )");

            // Anonymous type are friend of PetaPoco
            var xfile = new { FileName = "Agent Mulder.sec" };

            // Tell PetaPoco to insert it
            var id = DB.Insert("XFiles", "Id", true, xfile);

            // Update the poco
            xfile = new { FileName = "Agent Mulder.sec" };

            // Update the database
            DB.Update("XFiles", "Id", xfile);

            // Get a clone/copy from the DB
            // Note: Check out the name parameters - cool eh?
            var clone = DB.Query<dynamic>("SELECT * FROM [XFiles] WHERE [Id] = @Id", new { Id = id }).Single();

            // See, they're are the same
            id.ShouldBe((int) clone.Id);
            xfile.FileName.ShouldBe((string) clone.FileName);
        }

        [Fact]
        public void InsertDynamicUnknownPocoWithConventionalNaming()
        {
            // Create the table for our unknown but conventional POCO
            DB.Execute(@"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_SCHEMA = 'dbo' AND t.TABLE_NAME = 'XFiles')
	                         DROP TABLE dbo.[XFiles]

                         CREATE TABLE dbo.[XFiles] (
                             [Id] INT IDENTITY(1,1) PRIMARY KEY,
	                         [FileName] VARCHAR(255) NOT NULL
                         )");

            // Dynamics type are friend of PetaPoco
            dynamic xfile = new System.Dynamic.ExpandoObject();
            xfile.FileName = "Agent Mulder.sec";

            // Tell PetaPoco to insert it
            var id = DB.Insert("XFiles", "Id", true, (object) xfile);

            // Update the poco
            xfile.FileName = "Agent Mulder.sec";

            // Update the database
            DB.Update("XFiles", "Id", (object) xfile);

            // Get a clone/copy from the DB
            // Note: Check out the name parameters - cool eh?
            var clone = DB.Query<dynamic>("SELECT * FROM [XFiles] WHERE [Id] = @Id", new { Id = id }).Single();

            // See, they're are the same
            id.ShouldBe((int) clone.Id);
            ((string) xfile.FileName).ShouldBe((string) clone.FileName);
        }
    }
}