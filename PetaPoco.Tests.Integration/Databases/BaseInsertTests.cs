﻿// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseInsertTests : BaseDatabase
    {
        private readonly Note _note = new Note
        {
            Text = "PetaPoco's note",
            CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        private readonly Order _order = new Order
        {
            PoNumber = "Peta's Order",
            Status = OrderStatus.Accepted,
            CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc),
            CreatedBy = "Harry"
        };

        private readonly OrderLine _orderLine = new OrderLine
        {
            Quantity = 5,
            SellPrice = 4.99m,
            Status = OrderLineStatus.Pending
        };

        private readonly Person _person = new Person
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Dob = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc),
            Height = 180,
            Name = "Peta"
        };

        protected BaseInsertTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public void Insert_GivenPoco_ShouldBeValid()
        {
            var id = DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);

            _person.Id.ShouldBe(id);
            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_WhenInsertingRelatedPocosAndGivenPoco_ShouldInsertPocos()
        {
            DB.Insert(_person);
            _order.PersonId = _person.Id;
            DB.Insert(_order);
            _orderLine.OrderId = _order.Id;
            DB.Insert(_orderLine);

            var personOther = DB.Single<Person>(_person.Id);
            var orderOther = DB.Single<Order>(_order.Id);
            var orderLineOther = DB.Single<OrderLine>(_orderLine.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
            orderOther.ShouldNotBeNull();
            orderOther.ShouldBe(_order);
            orderLineOther.ShouldNotBeNull();
            orderLineOther.ShouldBe(_orderLine);
        }

        [Fact]
        public void Insert_GivenPocoTableNameAndColumnName_ShouldInsertPoco()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther =
                DB.Single<Person>($"SELECT * From {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0",
                    _person.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_WhenInsertingRelatedPocosGivenPocoTableNameAndColumnName_ShouldInsertPocos()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);
            _order.PersonId = _person.Id;
            DB.Insert("SpecificOrders", "Id", _order);
            _orderLine.OrderId = _order.Id;
            DB.Insert("SpecificOrderLines", "Id", _orderLine);

            var personOther =
                DB.Single<Person>($"SELECT * FROM {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0",
                    _person.Id);
            var orderOther =
                DB.Single<Order>($"SELECT * FROM {DB.Provider.EscapeTableName("SpecificOrders")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0",
                    _order.Id);
            var orderLineOther =
                DB.Single<OrderLine>($"SELECT * FROM {DB.Provider.EscapeTableName("SpecificOrderLines")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0",
                    _orderLine.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
            orderOther.ShouldNotBeNull();
            orderOther.ShouldBe(_order);
            orderLineOther.ShouldNotBeNull();
            orderLineOther.ShouldBe(_orderLine);
        }

        [Fact]
        public void Insert_GivenTableNameAndAnonymousType_ShouldInsertPoco()
        {
            var log = new { Description = "Test log", CreatedOn = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc) };

            var logId = DB.Insert("TransactionLogs", log);
            var otherLog = DB.Single<TransactionLog>($"SELECT * FROM {DB.Provider.EscapeTableName("TransactionLogs")}");

            logId.ShouldBeNull();
            otherLog.Description.ShouldBe(log.Description);
            otherLog.CreatedOn.ShouldBe(log.CreatedOn);
        }

        [Fact]
        public void Insert_GivenNullByteArray_ShouldNotThrow()
        {
            DB.Insert("BugInvestigation_10R9LZYK", "Id", true, new { TestColumn1 = (byte[]) null });
            DB.ExecuteScalar<int>($"SELECT * FROM {DB.Provider.EscapeTableName("BugInvestigation_10R9LZYK")}").ShouldBe(1);
        }

        [Fact]
        public void Insert_GivenNonNullByteArray_ShouldNotThrow()
        {
            DB.Insert("BugInvestigation_10R9LZYK", "Id", true, new { TestColumn1 = new byte[] { 1, 2, 3 } });
            DB.ExecuteScalar<int>($"SELECT * FROM {DB.Provider.EscapeTableName("BugInvestigation_10R9LZYK")}").ShouldBe(1);
        }

        [Fact]
        public void Insert_GivenPocoWithNullDateTime_ShouldNotThrow()
        {
            _person.Dob = null;
            var id = DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);

            _person.Id.ShouldBe(id);
            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenConventionalPocoWithTable_ShouldInsertPoco()
        {
            var id = DB.Insert("People", _person);

            var otherPerson = DB.Single<Person>(id);

            _person.Id.ShouldBe(id);
            otherPerson.ShouldNotBeNull();
            otherPerson.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenConventionalPocoWithTableAndPrimaryKey_ShouldInsertPoco()
        {
            var id = DB.Insert("People", "Id", _person);

            var otherPerson = DB.Single<Person>(id);

            _person.Id.ShouldBe(id);
            otherPerson.ShouldNotBeNull();
            otherPerson.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenPocoWithPrimaryNullableValueType_ShouldBeValid()
        {
            var note = new NoteNullablePrimary() { Text = _note.Text, CreatedOn = _note.CreatedOn };

            var id = DB.Insert(note);

            var noteOther = DB.Single<NoteNullablePrimary>(note.Id);

            note.Id.ShouldBe(id);
            noteOther.ShouldNotBeNull();
            noteOther.ShouldBe(note);
        }

        [Fact]
        public void Insert_GivenTableNamePrimaryKeyNameAndAnonymousType_ShouldInsertPoco()
        {
            var note = new { Text = "Test note", CreatedOn = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc) };

            var id = DB.Insert("Note", "Id", note);

            var otherNote = DB.Single<Note>(id);

            otherNote.Text.ShouldBe(note.Text);
            otherNote.CreatedOn.ShouldBe(note.CreatedOn);
        }

        [Fact]
        public void Insert_GivenTableNamePrimaryKeyNameAndAnonymousTypeWithNullablePrimaryKey_ShouldInsertPoco()
        {
            var note = new { Id = (int?) null, Text = "Test note", CreatedOn = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc) };

            var id = DB.Insert("Note", "Id", note);
            var otherNote = DB.Single<Note>(id);

            id.ShouldNotBeNull();
            otherNote.Text.ShouldBe(note.Text);
            otherNote.CreatedOn.ShouldBe(note.CreatedOn);
        }

        [Fact]
        public void Insert_GivenTableNamePrimaryKeyNameAndAnonymousTypeWithStaticPrimaryKey_ShouldInsertPoco()
        {
            var person = new { Id = Guid.NewGuid(), Age = 18, Dob = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc), Height = 180, FullName = "Peta" };

            var id = DB.Insert("People", "Id", person);

            var otherPerson = DB.Single<Person>(id);

            id.ShouldNotBeNull();
            otherPerson.Id.ShouldBe(person.Id);
            otherPerson.Age.ShouldBe(person.Age);
            otherPerson.Dob.ShouldBe(person.Dob);
            otherPerson.Height.ShouldBe(person.Height);
            otherPerson.Name.ShouldBe(person.FullName);
        }
    }
}