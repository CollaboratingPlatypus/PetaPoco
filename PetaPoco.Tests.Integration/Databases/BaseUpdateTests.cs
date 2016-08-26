// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseUpdateTests : BaseDatabase
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
            Status = OrderLineStatus.Pending
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
            // Arrange
            DB.Insert(_person);
            _order.PersonId = _person.Id;
            DB.Insert(_order);
            _orderLine.OrderId = _order.Id;
            DB.Insert(_orderLine);

            // Act
            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            var orderOther = DB.Single<Order>(_order.Id);
            UpdateProperties(orderOther);
            DB.Update(orderOther).ShouldBe(1);
            orderOther = DB.Single<Order>(_order.Id);

            var orderLineOther = DB.Single<OrderLine>(_orderLine.Id);
            UpdateProperties(orderLineOther);
            DB.Update(orderLineOther).ShouldBe(1);
            orderLineOther = DB.Single<OrderLine>(_orderLine.Id);

            // Assert
            personOther.ShouldNotBe(_person, true);
            orderOther.ShouldNotBe(_order, true);
            orderLineOther.ShouldNotBe(_orderLine, true);
        }

        [Fact]
        public void Update_GivenPocoAndNullColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, (System.Collections.Generic.IEnumerable<string>)null).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenPocoColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, new string[] { "FullName", "Height" }.AsEnumerable()).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }

        [Fact]
        public void Update_GivenPocoAndPrimaryKey_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, _person.Id).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenPocoPrimaryKeyAndNullColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, _person.Id, null).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenPocoPrimaryKeyAndColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, _person.Id, new[] { "FullName", "Height" }).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }

        [Fact]
        public void Update_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValue_ShouldBeValid()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther = SinglePersonOther(_person.Id);
            UpdateProperties(personOther);
            DB.Update("SpecificPeople", "Id", personOther, _person.Id).ShouldBe(1);
            personOther = SinglePersonOther(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndNullColumns_ShouldBeValid()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther = SinglePersonOther(_person.Id);
            UpdateProperties(personOther);
            DB.Update("SpecificPeople", "Id", personOther, _person.Id, null).ShouldBe(1);
            personOther = SinglePersonOther(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndColumns_ShouldBeValid()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther = SinglePersonOther(_person.Id);
            UpdateProperties(personOther);
            DB.Update("SpecificPeople", "Id", personOther, _person.Id, new[] { "FullName", "Height" }).ShouldBe(1);
            personOther = SinglePersonOther(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }

        [Fact]
        public void Update_GivenSqlAndParameters_ShouldBeValid()
        {
            DB.Insert(_person);
            var pd = PocoData.ForType(_person.GetType(), new ConventionMapper());
            var sql = $"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey[0])} = @0";

            DB.Update<Person>(sql, _person.Id, "Feta's Order").ShouldBe(1);
            var personOther = DB.Single<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Name.ShouldNotBe(_person.Name);
        }

        [Fact]
        public void Update_GivenPocoUpdateSql_ShouldUpdateThePoco()
        {
            DB.Insert(_person);
            var pd = PocoData.ForType(_person.GetType(), new ConventionMapper());
            var sql = new Sql($"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                              $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey[0])} = @0", _person.Id, "Feta's Order");

            DB.Update<Person>(sql);
            var personOther = DB.Single<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Name.ShouldNotBe(_person.Name);
        }

        [Fact]
        public void Update_GivenNoPocoExists_ShouldBeValid()
        {
            var rowEffected = DB.Update(_person);

            rowEffected.ShouldBe(0);
        }

        [Fact]
        public void Update_GivenTablePrimaryKeyNameAndAnonymousType_ShouldBeValid()
        {
            DB.Insert(_person);

            DB.Update("People", "Id", new { _person.Id, FullName = "Feta", Age = 19, Dob = new DateTime(1946, 1, 12, 5, 9, 4, DateTimeKind.Utc), Height = 190 })
                .ShouldBe(1);
            var personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenTablePrimaryKeyNameAnonymousTypeAndPrimaryKeyValue_ShouldBeValid()
        {
            DB.Insert(_person);

            DB.Update("People", "Id", new { FullName = "Feta", Age = 19, Dob = new DateTime(1946, 1, 12, 5, 9, 4, DateTimeKind.Utc), Height = 190 }, _person.Id)
                .ShouldBe(1);
            var personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        private Person SinglePersonOther(Guid id)
        {
            return DB.Single<Person>($"SELECT * From {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", id);
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
            orderLine.Status = OrderLineStatus.Allocated;
        }
    }
}