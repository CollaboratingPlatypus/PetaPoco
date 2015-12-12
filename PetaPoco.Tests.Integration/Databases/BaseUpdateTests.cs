using System;
using PetaPoco.Tests.Integration.Models;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseUpdateTests : BaseDatabaseTest
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

        protected BaseUpdateTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public void Update_GivenPoco_ShouldBeValid()
        {
            DB.Insert(_person);
            _order.PersonId = _person.Id;
            DB.Insert(_order);
            _orderLine.OrderId = _order.Id;
            DB.Insert(_orderLine);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther);
            var orderOther = DB.Single<Order>(_order.Id);
            UpdateProperties(orderOther);
            DB.Update(orderOther);
            var orderLineOther = DB.Single<OrderLine>(_orderLine.Id);
            UpdateProperties(orderLineOther);
            DB.Update(orderLineOther);

            personOther.ShouldNotBe(_person, true);
            orderOther.ShouldNotBe(_order, true);
            orderLineOther.ShouldNotBe(_orderLine, true);
        }

        private static void UpdateProperties(Person person)
        {
            person.Name = "Feta";
            person.Age = 19;
            person.Dob = new DateTime(1946, 1, 12, 5, 9, 4, DateTimeKind.Utc);
            person.Height = 190;
        }

        private static void UpdateProperties(Order order)
        {
            order.PoNumber = "Feta's Order";
            order.Status = OrderStatus.Pending;
            order.CreatedOn = new DateTime(1949, 1, 11, 4, 2, 4, DateTimeKind.Utc);
            order.CreatedBy = "Jen";
        }

        private static void UpdateProperties(OrderLine orderLine)
        {
            orderLine.Quantity = 6;
            orderLine.SellPrice = 5.99m;
        }
    }
}