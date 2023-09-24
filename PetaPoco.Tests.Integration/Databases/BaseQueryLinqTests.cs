using System;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseQueryLinqTests : BaseDatabase
    {
        // TODO: Move to base class, combine with other test data
        #region Test Data

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

        #endregion

        protected BaseQueryLinqTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public virtual void Single_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            DB.Single<Person>(pk).ShouldNotBeNull();
        }

        [Fact]
        public virtual void Single_GivenPrimaryKeyMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>(Guid.NewGuid()));
        }

        [Fact]
        public virtual void Single_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void Single_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual void Single_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual void Single_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void Single_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual void Single_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual void SingleOrDefault_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            DB.SingleOrDefault<Person>(pk).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleOrDefault_GivenPrimaryKeyMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>(Guid.NewGuid()).ShouldBeNull();
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.SingleOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeNull();
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.SingleOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual void SingleOrDefault_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public virtual void First_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void First_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void First_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual void First_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void First_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void First_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.FirstOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.FirstOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstOrDefault_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public virtual void Exists_GivenPrimaryKeyMatchingOneRecord_ShouldBeTrue()
        {
            var pk = DB.Insert(_person);
            DB.Exists<Person>(pk).ShouldBeTrue();
        }

        [Fact]
        public virtual void Exists_GivenPrimaryKeyMatchingNoRecord_ShouldBeFalse()
        {
            DB.Exists<Person>(Guid.NewGuid()).ShouldBeFalse();
        }

        [Fact(DisplayName = "Support the older syntax of starting with a WHERE clause.")]
        [Trait("Category", "Regression")]
        public virtual void Exists_Regression_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            DB.Exists<Person>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public virtual void Exists_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public virtual void Exists_GivenSqlStringMatchingMoreThanOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public virtual void Exists_GivenSqlStringMatchingNoRecord_ShouldBeFalse()
        {
            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeFalse();
        }

        [Fact]
        public virtual async void ExistsAsync_GivenPrimaryKeyMatchingOneRecord_ShouldBeTrue()
        {
            var pk = DB.Insert(_person);
            (await DB.ExistsAsync<Person>(pk)).ShouldBeTrue();
        }

        [Fact]
        public virtual async void ExistsAsync_GivenPrimaryKeyMatchingNoRecord_ShouldBeFalse()
        {
            (await DB.ExistsAsync<Person>(Guid.NewGuid())).ShouldBeFalse();
        }

        [Fact(DisplayName = "Support the older syntax of starting with a WHERE clause.")]
        [Trait("Category", "Regression")]
        public virtual async void ExistsAsync_Regression_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            (await DB.ExistsAsync<Person>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeTrue();
        }

        [Fact]
        public virtual async void ExistsAsync_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            (await DB.ExistsAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeTrue();
        }

        [Fact]
        public virtual async void ExistsAsync_GivenSqlStringMatchingMoreThanOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            (await DB.ExistsAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeTrue();
        }

        [Fact]
        public virtual async void ExistsAsync_GivenSqlStringMatchingNoRecord_ShouldBeFalse()
        {
            (await DB.ExistsAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeFalse();
        }

        [Fact]
        public virtual async void SingleAsync_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            (await DB.SingleAsync<Person>(pk)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleAsync_GivenPrimaryKeyMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(DB.SingleAsync<Person>(Guid.NewGuid()));
        }

        [Fact]
        public virtual async void SingleAsync_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.SingleAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleAsync_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(DB.SingleAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual void SingleAsync_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(DB.SingleAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual async void SingleAsync_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.SingleAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleAsync_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(DB.SingleAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual void SingleAsync_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(DB.SingleAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            (await DB.SingleOrDefaultAsync<Person>(pk)).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenPrimaryKeyMatchingNoRecord_ShouldBeNull()
        {
            (await DB.SingleOrDefaultAsync<Person>(Guid.NewGuid())).ShouldBeNull();
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.SingleOrDefaultAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleOrDefaultAsync_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(DB.SingleAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            (await DB.SingleOrDefaultAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.SingleOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual void SingleOrDefaultAsync_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(DB.SingleAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual async void SingleOrDefaultAsync_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            (await DB.SingleOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldBeNull();
        }

        [Fact]
        public virtual async void FirstAsync_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.FirstAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstAsync_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            (await DB.FirstAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstAsync_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(DB.FirstAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public virtual async void FirstAsync_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.FirstAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstAsync_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            (await DB.FirstAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual void FirstAsync_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(DB.FirstAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.FirstOrDefaultAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            (await DB.FirstOrDefaultAsync<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            (await DB.FirstOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldBeNull();
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            (await DB.FirstOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            (await DB.FirstOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldNotBeNull();
        }

        [Fact]
        public virtual async void FirstOrDefaultAsync_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            (await DB.FirstOrDefaultAsync<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18))).ShouldBeNull();
        }
    }
}
