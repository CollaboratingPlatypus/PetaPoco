// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.CodeDom;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseInsertTests : BaseDatabaseTest
    {
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

        protected BaseInsertTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public void Insert_GivenPoco_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenPoco_WhenInsertingRelatedPocosShouldBeValid()
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
        public void Insert_GivenPocoTableNameAndColumnName_ShouldBeValid()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther =
                DB.Single<Person>($"SELECT * From {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0",
                    _person.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenPocoTableNameAndColumnName_WhenInsertingRelatedPocosShouldBeValid()
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
                DB.Single<Order>($"SELECT * FROM {DB.Provider.EscapeTableName("SpecificOrders")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _order.Id);
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
        public void Insert_GivenTableNamePrimaryKeyNameAndAnonymousType_ShouldBeValid()
        {
            var note = new { Text = "Test note", CreatedOn = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc) };

            var key = DB.Insert("Note", "Id", note);
            var otherNote = DB.Single<Note>(key);

            key.ShouldNotBeNull();
            otherNote.Text.ShouldBe(note.Text);
            otherNote.CreatedOn.ShouldBe(note.CreatedOn);
        }

        [Fact]
        public void Insert_GivenTableNameAndAnonymousType_ShouldBeValid()
        {
            var log = new { Description = "Test log", CreatedOn = new DateTime(1945, 1, 12, 5, 9, 4, DateTimeKind.Utc) };

            var logKey = DB.Insert("TransactionLogs", log);
            var otherLog = DB.Single<TransactionLog>($"SELECT * FROM {DB.Provider.EscapeTableName("TransactionLogs")}");

            logKey.ShouldBeNull();
            otherLog.Description.ShouldBe(log.Description);
            otherLog.CreatedOn.ShouldBe(log.CreatedOn);
        }
    }
}