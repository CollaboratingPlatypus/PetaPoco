using System;
using PetaPoco.Tests.Integration.Models.SqlServer;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServerMSData
{
    [Collection("SqlServerMSData")]
    public class SqlServerMSDataQueryLinqTests : QueryLinqTests
    {
        #region Test Data

        private readonly StorePerson _storePerson = new StorePerson
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Name = "Peta"
        };

        #endregion

        public SqlServerMSDataQueryLinqTests()
            : base(new SqlServerMSDataDbProviderFactory())
        {
        }

        [Fact]
        [Trait("Issue", "#242")]
        [Trait("DBFeature", "Schema")]
        public void Exists_GivenPrimaryKeyMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            var pk = DB.Insert(_storePerson);
            DB.Exists<StorePerson>(pk).ShouldBeTrue();
        }

        [Fact]
        [Trait("Issue", "#242")]
        [Trait("DBFeature", "Schema")]
        public void Exists_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            DB.Insert(_storePerson);
            DB.Exists<StorePerson>($"WHERE {DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }

        [Fact(DisplayName = "Exists: Support the older syntax of starting with a WHERE clause.")]
        [Trait("Category", "Regression")]
        [Trait("Issue", "#237")]
        [Trait("Issue", "#238")]
        [Trait("Issue", "#242")]
        [Trait("DBFeature", "Schema")]
        public void Exists_Regression_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            DB.Insert(_storePerson);
            DB.Exists<StorePerson>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }
    }
}
