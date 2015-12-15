// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDeleteTests : BaseDatabase
    {
        private TransactionLog _log = new TransactionLog
        {
            Description = "A test log",
            CreatedOn = new DateTime(1957, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        private Note _note = new Note
        {
            Text = "A test note",
            CreatedOn = new DateTime(1955, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        private Note _note2 = new Note
        {
            Text = "A test note 2",
            CreatedOn = new DateTime(1985, 1, 11, 4, 2, 4, DateTimeKind.Utc)
        };

        private Order _order = new Order
        {
            PoNumber = "Peta's Order",
            Status = OrderStatus.Accepted,
            CreatedOn = new DateTime(1948, 1, 11, 4, 2, 4, DateTimeKind.Utc),
            CreatedBy = "Harry"
        };

        private OrderLine _orderLine = new OrderLine
        {
            Quantity = 5,
            SellPrice = 4.99m,
        };

        private Person _person = new Person
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Dob = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc),
            Height = 180,
            Name = "Peta"
        };

        protected BaseDeleteTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public void Delete_GivenPoco_ShouldBeValid()
        {
            // Arrange
            DB.Insert(_person);
            _order.PersonId = _person.Id;
            DB.Insert(_order);
            _orderLine.OrderId = _order.Id;
            DB.Insert(_orderLine);
            DB.Insert(_note);

            // Act
            DB.Delete(_orderLine);
            DB.Delete(_order);
            DB.Delete(_person);
            DB.Delete(_note);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _order = DB.SingleOrDefault<Order>(_order.Id);
            _orderLine = DB.SingleOrDefault<OrderLine>(_orderLine.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);

            // Assert
            _person.ShouldBeNull();
            _order.ShouldBeNull();
            _orderLine.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public void Delete_GivenPocoOrPrimaryKey_IsValid()
        {
            DB.Insert(_note);
            DB.Insert(_note2);
            DB.Insert(_person);

            DB.Delete<Person>(_person.Id).ShouldBe(1);
            DB.Delete<Note>(_note).ShouldBe(1);
            DB.Delete<Note>(new { _note2.Id }).ShouldBe(1);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);
            _note2 = DB.SingleOrDefault<Note>(_note2.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
            _note2.ShouldBeNull();
        }

        [Fact]
        public void Delete_GivenTableNamePrimaryKeyNameAndPoco_IsValid()
        {
            DB.Insert(_person);
            DB.Insert(_note);

            DB.Delete("People", "Id", _person).ShouldBe(1);
            DB.Delete("Note", "Id", _note).ShouldBe(1);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public void Delete_GivenTableNamePrimaryKeyNamePocoAndPrimaryKeyValue_IsValid()
        {
            DB.Insert(_person);
            DB.Insert(_note);

            DB.Delete("People", "Id", _person, _person.Id).ShouldBe(1);
            DB.Delete("Note", "Id", _note, _note.Id).ShouldBe(1);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public void Delete_GivenSqlAndArgs_IsValid()
        {
            DB.Insert(_note);
            DB.Insert(_person);

            DB.Delete<Note>($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _note.Id).ShouldBe(1);
            DB.Delete<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _person.Id).ShouldBe(1);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public void Delete_GivenSql_IsValid()
        {
            DB.Insert(_note);
            DB.Insert(_person);

            DB.Delete<Note>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _note.Id)).ShouldBe(1);
            DB.Delete<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _person.Id)).ShouldBe(1);

            _person = DB.SingleOrDefault<Person>(_person.Id);
            _note = DB.SingleOrDefault<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }
    }
}