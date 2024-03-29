﻿using System;
using PetaPoco.Tests.Integration.Models.SqlServer;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.SqlServer
{
    public abstract partial class SqlServerQueryLinqTests : QueryLinqTests
    {
        #region Test Data

        private readonly StorePerson _storePerson = new StorePerson
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Name = "Peta"
        };

        #endregion

        protected SqlServerQueryLinqTests(TestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        [Trait("Issue", "#242")]
        [Trait("DBFeature", "Schema")]
        public virtual void Exists_GivenPrimaryKeyMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            var pk = DB.Insert(_storePerson);
            DB.Exists<StorePerson>(pk).ShouldBeTrue();
        }

        [Fact]
        [Trait("Issue", "#242")]
        [Trait("DBFeature", "Schema")]
        public virtual void Exists_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
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
        public virtual void Exists_Regression_GivenSqlStringMatchingOneRecordAndPocoWithSchema_ShouldBeTrue()
        {
            DB.Insert(_storePerson);
            DB.Exists<StorePerson>($"{DB.Provider.EscapeSqlIdentifier("Age")} = @0", 18).ShouldBeTrue();
        }


        [Collection("SqlServer.SystemData")]
        public class SystemData : SqlServerQueryLinqTests
        {
            public SystemData()
                : base(new SqlServerSystemDataTestProvider())
            {
            }
        }

        [Collection("SqlServer.MicrosoftData")]
        public class MicrosoftData : SqlServerQueryLinqTests
        {
            public MicrosoftData()
                : base(new SqlServerMSDataTestProvider())
            {
            }
        }
    }
}
