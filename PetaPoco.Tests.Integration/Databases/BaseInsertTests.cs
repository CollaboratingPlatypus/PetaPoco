// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using System;
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
        public void Insert_GivenNull_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.Insert(null));
        }

        [Fact]
        public void Insert_GivenPoco_IsValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBeNull();
            personOther.ShouldBe(_person);
        }

        [Fact]
        public void Insert_GivenPoco_WhenInsertingRelatedPocosIsValid()
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
    }
}