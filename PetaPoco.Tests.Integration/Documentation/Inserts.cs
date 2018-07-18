﻿// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

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
    public class Inserts : BaseDatabase
    {
        public Inserts()
            : base(new MssqlDBTestProvider())
        {
            PocoData.FlushCaches();
        }

        [Fact]
        public void Insert()
        {
            // Create the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };

            // Tell PetaPoco to insert it
            var id = DB.Insert(person);

            // Obviously the ID returned will be the same at the one we set
            id.ShouldBe(person.Id);

            // Get a clone/copy from the DB
            var clone = DB.Single<Person>(id);

            // See, they're are the same
            clone.ShouldBe(person);

            // But, they're not not reference equals as PetaPoco doesn't cache because it's a MircoORM.
            person.Equals(clone).ShouldBeFalse();
        }

        [Fact]
        public void InsertAutoIncrement()
        {
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };
            DB.Insert(person);

            // Create the order
            var order = new Order
            {
                PersonId = person.Id,
                PoNumber = "PETAPOCO",
                Status = OrderStatus.Pending,
                CreatedBy = "Office PetaPoco",
                CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc)
            };

            // Tell PetaPoco to insert it
            var id = DB.Insert(order);

            // PetaPoco updates the POCO's ID for us, see
            id.ShouldBe(order.Id);

            // Get a clone/copy from the DB
            var clone = DB.Single<Order>(id);

            // See, they're are the same
            clone.ShouldBe(order);

            // But, they're not not reference equals as PetaPoco doesn't cache because it's a MircoORM.
            order.Equals(clone).ShouldBeFalse();
        }

        [Fact]
        public void InsertToDifferentTable()
        {
            // Create the person
            var person = new Person { Id = Guid.NewGuid(), Name = "PetaPoco", Dob = new DateTime(2011, 1, 1), Age = (DateTime.Now.Year - 2011), Height = 242 };

            // Tell PetaPoco to insert it, but to the "SpecificPeople" table and not the "People" table.
            // Note: the id will only be returned if PetaPoco can tell which is the primary key via mapping or convention.
            var id = DB.Insert("SpecificPeople", person);

            // Obviously the ID returned will be the same at the one we set
            id.ShouldBe(person.Id);

            // Get a clone/copy from "People" table (Default table as per mappings)
            var clone = DB.SingleOrDefault<Person>(id);

            // As expected, doesn't exist
            clone.ShouldBeNull();

            // We need to get the clone/copy from the correct table
            // Note: we can't use auto select builder here because PetaPoco would create columns such as People.Id
            clone = DB.Query<Person>("SELECT * FROM [SpecificPeople] sp WHERE sp.[Id] = @0", id).Single();

            // See, they're are the same
            clone.ShouldBe(person);

            // But, they're not not reference equals as PetaPoco doesn't cache because it's a MircoORM.
            person.Equals(clone).ShouldBeFalse();
        }

        [Fact]
        public void InsertConventionalPoco()
        {
            // Clear out any notes and reset the ID sequence counter
            DB.Execute("TRUNCATE TABLE [Note]");

            // Insert some notes using all APIs

            // Each of the API usuages here are effectively the same, as PetaPoco is providing the correct unknown values. 
            // This is because the poco has been mapped by convention and therefore PetaPoco understands how to do this.
            var id1 = DB.Insert(new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) });
            var id2 = DB.Insert("Note", new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) });
            var id3 = DB.Insert("Note", "Id", new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) });
            var id4 = DB.Insert("Note", "Id", true, new Note { Text = "PetaPoco's note", CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc) });

            // Am I right?
            id1.ShouldBe(1);
            id2.ShouldBe(2);
            id3.ShouldBe(3);
            id4.ShouldBe(4);

            // Just to be sure
            DB.ExecuteScalar<int>("SELECT COUNT(*) FROM [Note] WHERE CAST(Text AS NVARCHAR(MAX)) = @0", "PetaPoco's note").ShouldBe(4);
        }

        [Fact]
        public void InsertUnconventionalPoco()
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

            // Get a clone/copy from the DB
            var clone = DB.Query<UnconventionalPoco>("SELECT * FROM [TBL_UnconventionalPocos] WHERE [PrimaryKey] = @0", id).Single();

            // See, they're are the same
            clone.ShouldBe(poco);

            // But, they're not not reference equals as PetaPoco doesn't cache because it's a MircoORM.
            poco.Equals(clone).ShouldBeFalse();
        }

        [Fact]
        public void InsertConventionalUnconventionalPoco()
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

            // But, they're not not reference equals as PetaPoco doesn't cache because it's a MircoORM.
            poco.Equals(clone).ShouldBeFalse();
        }

        [Fact]
        public void InsertAnonymousPocoWithConventionalNaming()
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

            // Get a clone/copy from the DB
            // Note: Check out the name parameters - cool eh?
            var clone = DB.Query<dynamic>("SELECT * FROM [XFiles] WHERE [Id] = @Id", new { Id = id }).Single();

            // See, they're are the same
            id.ShouldBe((int) clone.Id);
            ((string) xfile.FileName).ShouldBe((string) clone.FileName);
        }
    }

    public class UnconventionalPoco
    {
        public int PrimaryKey { get; set; }

        public string Text { get; set; }

        public void ShouldBe(UnconventionalPoco other)
        {
            PrimaryKey.ShouldBe(other.PrimaryKey);
            Text.ShouldBe(other.Text);
        }
    }

    [TableName("People")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class Person
    {
        [Column]
        public Guid Id { get; set; }

        [Column(Name = "FullName")]
        public string Name { get; set; }

        [Column]
        public long Age { get; set; }

        [Column]
        public int Height { get; set; }

        [Column]
        public DateTime? Dob { get; set; }

        [Ignore]
        public string NameAndAge => $"{Name} is of {Age}";

        public void ShouldBe(Person other)
        {
            Id.ShouldBe(other.Id);
            Name.ShouldBe(other.Name);
            Age.ShouldBe(other.Age);
            Height.ShouldBe(other.Height);
            Dob.ShouldBe(other.Dob);
        }
    }

    public class Note
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }
    }

    [ExplicitColumns]
    [TableName("Orders")]
    [PrimaryKey("Id")]
    public class Order
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public Guid PersonId { get; set; }

        [Column]
        public string PoNumber { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [Column]
        public string CreatedBy { get; set; }

        [Column("OrderStatus")]
        public OrderStatus Status { get; set; }

        public void ShouldBe(Order other)
        {
            Id.ShouldBe(other.Id);
            PersonId.ShouldBe(other.PersonId);
            PoNumber.ShouldBe(other.PoNumber);
            Status.ShouldBe(other.Status);
            CreatedOn.ShouldBe(other.CreatedOn);
            CreatedBy.ShouldBe(other.CreatedBy);
        }
    }

    public enum OrderStatus
    {
        Pending,
        Accepted,
        Rejected,
        Deleted
    }

    [TableName("OrderLines")]
    [PrimaryKey("Id")]
    public class OrderLine
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int OrderId { get; set; }

        [Column(Name = "Qty")]
        public short Quantity { get; set; }

        [Column]
        public decimal SellPrice { get; set; }

        [ResultColumn]
        public decimal Total { get; set; }
    }
}