using System;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Core;
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
            DB.Update(personOther, null).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public void Update_GivenPocoColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, new[] { "FullName", "Height" }).ShouldBe(1);
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
        public void Update_GivenPocoAndEmptyColumns_ShouldBeValid()
        {
            DB.Insert(_person);

            var personOther = DB.Single<Person>(_person.Id);
            UpdateProperties(personOther);
            DB.Update(personOther, new string[0]).ShouldBe(0);
            personOther = DB.Single<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldBe(_person.Name);
            personOther.Height.ShouldBe(_person.Height);
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
        public void Update_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndEmptyColumns_ShouldBeValid()
        {
            DB.Insert("SpecificPeople", "Id", false, _person);

            var personOther = SinglePersonOther(_person.Id);
            UpdateProperties(personOther);
            DB.Update("SpecificPeople", "Id", personOther, _person.Id, new string[0]).ShouldBe(0);
            personOther = SinglePersonOther(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldBe(_person.Name);
            personOther.Height.ShouldBe(_person.Height);
        }

        [Fact]
        public void Update_GivenSqlAndParameters_ShouldBeValid()
        {
            DB.Insert(_person);
            var pd = PocoData.ForType(_person.GetType(), new ConventionMapper());
            var sql = $"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey)} = @0";

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
            var sql = new Sql(
                $"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey)} = @0", _person.Id, "Feta's Order");

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

        [Fact]
        public async Task UpdateAsync_GivenPoco_ShouldBeValid()
        {
            // Arrange
            await DB.InsertAsync(_person);
            _order.PersonId = _person.Id;
            await DB.InsertAsync(_order);
            _orderLine.OrderId = _order.Id;
            await DB.InsertAsync(_orderLine);

            // Act
            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther)).ShouldBe(1);
            personOther = DB.Single<Person>(_person.Id);

            var orderOther = await DB.SingleAsync<Order>(_order.Id);
            UpdateProperties(orderOther);
            (await DB.UpdateAsync(orderOther)).ShouldBe(1);
            orderOther = await DB.SingleAsync<Order>(_order.Id);

            var orderLineOther = await DB.SingleAsync<OrderLine>(_orderLine.Id);
            UpdateProperties(orderLineOther);
            (await DB.UpdateAsync(orderLineOther)).ShouldBe(1);
            orderLineOther = await DB.SingleAsync<OrderLine>(_orderLine.Id);

            // Assert
            personOther.ShouldNotBe(_person, true);
            orderOther.ShouldNotBe(_order, true);
            orderLineOther.ShouldNotBe(_orderLine, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoAndNullColumns_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, null)).ShouldBe(1);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoColumns_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, new[] { "FullName", "Height" })).ShouldBe(1);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoAndPrimaryKey_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, _person.Id)).ShouldBe(1);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoPrimaryKeyAndNullColumns_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, _person.Id, null)).ShouldBe(1);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoPrimaryKeyAndColumns_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, _person.Id, new[] { "FullName", "Height" })).ShouldBe(1);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }
        [Fact]
        public async Task UpdateAsync_GivenPocoAndEmptyColumns_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            var personOther = await DB.SingleAsync<Person>(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync(personOther, new string[0])).ShouldBe(0);
            personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldBe(_person.Name);
            personOther.Height.ShouldBe(_person.Height);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValue_ShouldBeValid()
        {
            await DB.InsertAsync("SpecificPeople", "Id", false, _person);

            var personOther = await SinglePersonOtherAsync(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync("SpecificPeople", "Id", personOther, _person.Id)).ShouldBe(1);
            personOther = await SinglePersonOtherAsync(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndNullColumns_ShouldBeValid()
        {
            await DB.InsertAsync("SpecificPeople", "Id", false, _person);

            var personOther = await SinglePersonOtherAsync(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync("SpecificPeople", "Id", personOther, _person.Id, null)).ShouldBe(1);
            personOther = await SinglePersonOtherAsync(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndColumns_ShouldBeValid()
        {
            await DB.InsertAsync("SpecificPeople", "Id", false, _person);

            var personOther = await SinglePersonOtherAsync(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync("SpecificPeople", "Id", personOther, _person.Id, new[] { "FullName", "Height" })).ShouldBe(1);
            personOther = await SinglePersonOtherAsync(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldNotBe(_person.Name);
            personOther.Height.ShouldNotBe(_person.Height);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNamePocoAndPrimaryKeyValueAndEmptyColumns_ShouldBeValid()
        {
            await DB.InsertAsync("SpecificPeople", "Id", false, _person);

            var personOther = await SinglePersonOtherAsync(_person.Id);
            UpdateProperties(personOther);
            (await DB.UpdateAsync("SpecificPeople", "Id", personOther, _person.Id, new string[0])).ShouldBe(0);
            personOther = await SinglePersonOtherAsync(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Age.ShouldBe(_person.Age);
            personOther.Dob.ShouldBe(_person.Dob);
            personOther.Name.ShouldBe(_person.Name);
            personOther.Height.ShouldBe(_person.Height);
        }

        [Fact]
        public async Task UpdateAsync_GivenSqlAndParameters_ShouldBeValid()
        {
            await DB.InsertAsync(_person);
            var pd = PocoData.ForType(_person.GetType(), new ConventionMapper());
            var sql = $"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey)} = @0";

            (await DB.UpdateAsync<Person>(sql, _person.Id, "Feta's Order")).ShouldBe(1);
            var personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Name.ShouldNotBe(_person.Name);
        }

        [Fact]
        public async Task UpdateAsync_GivenPocoUpdateSql_ShouldUpdateThePoco()
        {
            await DB.InsertAsync(_person);
            var pd = PocoData.ForType(_person.GetType(), new ConventionMapper());
            var sql = new Sql(
                $"SET {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} = @1 " +
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.TableInfo.PrimaryKey)} = @0", _person.Id, "Feta's Order");

            await DB.UpdateAsync<Person>(sql);
            var personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.Id.ShouldBe(_person.Id);
            personOther.Name.ShouldNotBe(_person.Name);
        }

        [Fact]
        public async Task UpdateAsync_GivenNoPocoExists_ShouldBeValid()
        {
            var rowEffected = await DB.UpdateAsync(_person);

            rowEffected.ShouldBe(0);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNameAndAnonymousType_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            (await DB.UpdateAsync("People", "Id",
                new { _person.Id, FullName = "Feta", Age = 19, Dob = new DateTime(1946, 1, 12, 5, 9, 4, DateTimeKind.Utc), Height = 190 })).ShouldBe(1);
            var personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        [Fact]
        public async Task UpdateAsync_GivenTablePrimaryKeyNameAnonymousTypeAndPrimaryKeyValue_ShouldBeValid()
        {
            await DB.InsertAsync(_person);

            (await DB.UpdateAsync("People", "Id", new { FullName = "Feta", Age = 19, Dob = new DateTime(1946, 1, 12, 5, 9, 4, DateTimeKind.Utc), Height = 190 },
                _person.Id)).ShouldBe(1);
            var personOther = await DB.SingleAsync<Person>(_person.Id);

            personOther.ShouldNotBe(_person, true);
        }

        private Person SinglePersonOther(Guid id)
            => DB.Single<Person>($"SELECT * From {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", id);

        private Task<Person> SinglePersonOtherAsync(Guid id)
            => DB.SingleAsync<Person>($"SELECT * From {DB.Provider.EscapeTableName("SpecificPeople")} WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", id);

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