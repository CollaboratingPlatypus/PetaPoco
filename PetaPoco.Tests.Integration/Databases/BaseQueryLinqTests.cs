// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/20</date>

using System;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseQueryLinqTests : BaseDatabase
    {
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

        private Item _item = new Item
        {
            UserId = 1,
            Index = 10,
            Type = 20,
            Content = "abc"
        };

        protected BaseQueryLinqTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public void Single_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            DB.Single<Person>(pk).ShouldNotBeNull();

            var pk1 = (object[])DB.Insert(_item);
            DB.Single<Item>(pk1[0], pk1[1]).ShouldNotBeNull();
        }

        [Fact]
        public void Single_GivenPrimaryKeyMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>(Guid.NewGuid()));
            Should.Throw<Exception>(() => DB.Single<Item>(0, 0));
        }

        [Fact]
        public void Single_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void Single_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public void Single_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public void Single_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void Single_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public void Single_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public void SingleOrDefault_GivenPrimaryKeyMatchingOneRecord_ShouldReturnPoco()
        {
            var pk = DB.Insert(_person);
            DB.SingleOrDefault<Person>(pk).ShouldNotBeNull();

            var pk1 = (object[])DB.Insert(_item);
            DB.SingleOrDefault<Item>(pk1[0], pk1[1]).ShouldNotBeNull();
        }

        [Fact]
        public void SingleOrDefault_GivenPrimaryKeyMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>(Guid.NewGuid()).ShouldBeNull();
            DB.SingleOrDefault<Item>(0, 0).ShouldBeNull();
        }

        [Fact]
        public void SingleOrDefault_GivenSqlStringMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.SingleOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void SingleOrDefault_GivenSqlStringMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public void SingleOrDefault_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeNull();
        }

        [Fact]
        public void SingleOrDefault_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.SingleOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void SingleOrDefault_GivenSqlMatchingTwoRecords_ShouldThrow()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            Should.Throw<Exception>(() => DB.Single<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public void SingleOrDefault_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            DB.SingleOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public void First_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void First_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void First_GivenSqlStringMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.First<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18));
        }

        [Fact]
        public void First_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void First_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void First_GivenSqlMatchingNoRecord_ShouldThrow()
        {
            Should.Throw<Exception>(() => DB.First<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)));
        }

        [Fact]
        public void FirstOrDefault_GivenSqlStringAndMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.FirstOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void FirstOrDefault_GivenSqlStringAndMatchingTwoRecords_ShouldReturnFirstRecord()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.FirstOrDefault<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldNotBeNull();
        }

        [Fact]
        public void FirstOrDefault_GivenSqlStringMatchingNoRecord_ShouldBeNull()
        {
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public void FirstOrDefault_GivenSqlMatchingOneRecord_ShouldReturnPoco()
        {
            DB.Insert(_person);
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void FirstOrDefault_GivenSqlMatchingTwoRecords_ShouldReturnFirstPoco()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldNotBeNull();
        }

        [Fact]
        public void FirstOrDefault_GivenSqlMatchingNoRecord_ShouldBeNull()
        {
            DB.FirstOrDefault<Person>(new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18)).ShouldBeNull();
        }

        [Fact]
        public void Exists_GivenPrimaryKeyMatchingOneRecord_ShouldBeTrue()
        {
            var pk = DB.Insert(_person);
            DB.Exists<Person>(pk).ShouldBeTrue();

            var pk1 = (object[])DB.Insert(_item);
            DB.Exists<Item>(pk1[0], pk1[1]).ShouldBeTrue();
        }

        [Fact]
        public void Exists_GivenPrimaryKeyMatchingNoRecord_ShouldBeFalse()
        {
            DB.Exists<Person>(Guid.NewGuid()).ShouldBeFalse();
            DB.Exists<Item>(0, 0).ShouldBeFalse();
        }

        /// <summary>
        ///     Support the older syntax of starting witha WHERE clause.
        /// </summary>
        [Fact]
        public void Exists_Regression_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            DB.Exists<Person>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public void Exists_GivenSqlStringMatchingOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public void Exists_GivenSqlStringMatchingMoreThanOneRecord_ShouldBeTrue()
        {
            DB.Insert(_person);
            _person.Id = Guid.NewGuid();
            DB.Insert(_person);

            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact]
        public void Exists_GivenSqlStringMatchingNoRecord_ShouldBeFalse()
        {
            DB.Exists<Person>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeFalse();
        }
    }
}