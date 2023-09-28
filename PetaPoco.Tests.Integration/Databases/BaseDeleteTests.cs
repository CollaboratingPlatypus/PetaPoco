using System;
using System.Threading.Tasks;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDeleteTests : BaseDatabase
    {
        // TODO: Move to base class, combine with other test data
        #region Test Data

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

        #endregion

        protected BaseDeleteTests(DBTestProvider provider)
            : base(provider)
        {
        }

        // TODO: Test with/without "WHERE" keyword in sql string produce identical statements

        [Fact]
        public virtual void Delete_GivenPoco_ShouldDeletePoco()
        {
            // Arrange
            DB.Insert(_person);
            _order.PersonId = _person.Id;
            DB.Insert(_order);
            _orderLine.OrderId = _order.Id;
            DB.Insert(_orderLine);
            DB.Insert(_note);

            // TODO: assert Delete returns 1
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
        public virtual void Delete_GivenPocoOrPrimaryKey_ShouldDeletePoco()
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
        public virtual void Delete_GivenTableNamePrimaryKeyNameAndPoco_ShouldDeletePoco()
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
        public virtual void Delete_GivenTableNamePrimaryKeyNamePocoAndPrimaryKeyValue_ShouldDeletePoco()
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
        public virtual void Delete_GivenSqlAndArgs_ShouldDeletePoco()
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
        public virtual void Delete_GivenSql_ShouldDeletePoco()
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

        [Fact]
        public virtual async Task DeleteAsync_GivenPoco_ShouldDeletePoco()
        {
            // Arrange
            await DB.InsertAsync(_person);
            _order.PersonId = _person.Id;
            await DB.InsertAsync(_order);
            _orderLine.OrderId = _order.Id;
            await DB.InsertAsync(_orderLine);
            await DB.InsertAsync(_note);

            // TODO: assert DeleteAsync returns 1
            // Act
            await DB.DeleteAsync(_orderLine);
            await DB.DeleteAsync(_order);
            await DB.DeleteAsync(_person);
            await DB.DeleteAsync(_note);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _order = await DB.SingleOrDefaultAsync<Order>(_order.Id);
            _orderLine = await DB.SingleOrDefaultAsync<OrderLine>(_orderLine.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);

            // Assert
            _person.ShouldBeNull();
            _order.ShouldBeNull();
            _orderLine.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public virtual async Task DeleteAsync_GivenPocoOrPrimaryKey_ShouldDeletePoco()
        {
            await DB.InsertAsync(_note);
            await DB.InsertAsync(_note2);
            await DB.InsertAsync(_person);

            (await DB.DeleteAsync<Person>(_person.Id)).ShouldBe(1);
            (await DB.DeleteAsync<Note>(_note)).ShouldBe(1);
            (await DB.DeleteAsync<Note>(new { _note2.Id })).ShouldBe(1);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);
            _note2 = await DB.SingleOrDefaultAsync<Note>(_note2.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
            _note2.ShouldBeNull();
        }

        [Fact]
        public virtual async Task DeleteAsync_GivenTableNamePrimaryKeyNameAndPoco_ShouldDeletePoco()
        {
            await DB.InsertAsync(_person);
            await DB.InsertAsync(_note);

            (await DB.DeleteAsync("People", "Id", _person)).ShouldBe(1);
            (await DB.DeleteAsync("Note", "Id", _note)).ShouldBe(1);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public virtual async Task DeleteAsync_GivenTableNamePrimaryKeyNamePocoAndPrimaryKeyValue_ShouldDeletePoco()
        {
            await DB.InsertAsync(_person);
            await DB.InsertAsync(_note);

            (await DB.DeleteAsync("People", "Id", _person, _person.Id)).ShouldBe(1);
            (await DB.DeleteAsync("Note", "Id", _note, _note.Id)).ShouldBe(1);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public virtual async Task DeleteAsync_GivenSqlAndArgs_ShouldDeletePoco()
        {
            await DB.InsertAsync(_note);
            await DB.InsertAsync(_person);

            (await DB.DeleteAsync<Note>($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _note.Id)).ShouldBe(1);
            (await DB.DeleteAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _person.Id)).ShouldBe(1);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }

        [Fact]
        public virtual async Task DeleteAsync_GivenSql_ShouldDeletePoco()
        {
            await DB.InsertAsync(_note);
            await DB.InsertAsync(_person);

            (await DB.DeleteAsync<Note>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _note.Id))).ShouldBe(1);
            (await DB.DeleteAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Id")} = @0", _person.Id))).ShouldBe(1);

            _person = await DB.SingleOrDefaultAsync<Person>(_person.Id);
            _note = await DB.SingleOrDefaultAsync<Note>(_note.Id);

            _person.ShouldBeNull();
            _note.ShouldBeNull();
        }
    }
}
