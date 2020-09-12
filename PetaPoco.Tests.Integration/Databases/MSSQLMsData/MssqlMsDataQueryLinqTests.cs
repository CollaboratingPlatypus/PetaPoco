using System;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    public class MssqlMsDataQueryLinqTests : BaseQueryLinqTests
    {
        private readonly StorePerson _storePerson = new StorePerson
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Name = "Peta"
        };

        public MssqlMsDataQueryLinqTests()
            : base(new MssqlMsDataDBTestProvider())
        {
        }

        [Fact]
        public void Exists_GivenPrimaryKeyMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            var pk = DB.Insert(_storePerson);
            DB.Exists<StorePerson>(pk).ShouldBeTrue();
        }

        [Fact]
        public void Exists_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            DB.Insert(_storePerson);
            DB.Exists<StorePerson>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        /// <summary>
        ///     Support the older syntax of starting witha WHERE clause.
        /// </summary>
        [Fact]
        public void Exists_Regression_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            DB.Insert(_storePerson);
            DB.Exists<StorePerson>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [TableName("store.People")]
        [PrimaryKey("Id", AutoIncrement = false)]
        public class StorePerson
        {
            [Column]
            public Guid Id { get; set; }

            [Column(Name = "FullName")]
            public string Name { get; set; }

            [Column]
            public long Age { get; set; }
        }
    }
}