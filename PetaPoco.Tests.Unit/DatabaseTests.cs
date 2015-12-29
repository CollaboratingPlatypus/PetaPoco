// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/28</date>

using System;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using Moq;
using PetaPoco.Core;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class DatabaseTests
    {
        private DbProviderFactory _dbProviderFactory = new Mock<DbProviderFactory>(MockBehavior.Strict).Object;

        private IProvider _provider = new Mock<IProvider>(MockBehavior.Loose).Object;
        private IDatabase DB { get; set; }

        public DatabaseTests()
        {
            DB = new Database("cs", _provider);
        }

        [Fact]
        public void Construct_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<InvalidOperationException>(() => new Database());

            Should.Throw<ArgumentNullException>(() => new Database((IDbConnection) null));

            Should.Throw<ArgumentNullException>(() => new Database("some connection string", (IProvider) null));
            Should.Throw<ArgumentException>(() => new Database(null, _provider));

            Should.Throw<ArgumentException>(() => new Database((string) null));
            Should.Throw<InvalidOperationException>(() => new Database("some connection string"));

            Should.Throw<ArgumentException>(() => new Database(null, _dbProviderFactory));
            Should.Throw<ArgumentNullException>(() => new Database("some connection string", (DbProviderFactory) null));

            Should.Throw<ArgumentNullException>(() => new Database((IDatabaseBuildConfiguration) null));
            Should.Throw<InvalidOperationException>(() =>
            {
                try
                {
                    DatabaseConfiguration.Build().Create();
                }
                catch (Exception e)
                {
                    e.Message.ShouldContain("One or more connection strings");
                    throw;
                }
            });
            Should.Throw<InvalidOperationException>(() =>
            {
                try
                {
                    DatabaseConfiguration.Build().UsingConnectionString("cs").Create();
                }
                catch (Exception e)
                {
                    e.Message.ShouldContain("Both a connection string and provider are required");
                    throw;
                }
            });
        }

        [Fact]
        public void Update_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), 1));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), 1));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, 1));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), 1, null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), 1, null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, 1, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", (Person) null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, (object) null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, (object) null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update<Person>((string) null));

            Should.Throw<ArgumentNullException>(() => DB.Update<Person>((Sql) null));
        }

        [Fact]
        public void Insert_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.Insert(null));
            Should.Throw<ArgumentNullException>(() => DB.Insert(null, null));
            Should.Throw<ArgumentNullException>(() => DB.Insert(null, "SomeColumn", new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Insert("SomeTable", null, new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Insert("SomeTable", "SomeColumn", null));
        }

        [Fact]
        public void IsNew_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.IsNew(null));
            Should.Throw<ArgumentNullException>(() => DB.IsNew(null, null));

            Should.Throw<ArgumentException>(() => DB.IsNew("MissingId", new { }));

            Should.Throw<InvalidOperationException>(() => DB.IsNew(new TransactionLog()));
            Should.Throw<InvalidOperationException>(() => DB.IsNew(new EntityWithoutConventionId()));
            Should.Throw<InvalidOperationException>(() => DB.IsNew(new ExpandoObject()));
        }

        [Fact]
        public void IsNew_GivenTransientEntity_ShouldBeValid()
        {
            var guidEntity = new GenericIdEntity<Guid>();
            var guidNullableEntity = new GenericIdEntity<Guid?>();
            var intEntity = new GenericIdEntity<int>();
            var unsignedIntEntity = new GenericIdEntity<uint>();
            var shortEntity = new GenericIdEntity<short>();
            var unsignedShortEntity = new GenericIdEntity<ushort>();
            var longEntity = new GenericIdEntity<long>();
            var unsignedLongEntity = new GenericIdEntity<ulong>();
            var stringEntity = new GenericIdEntity<string>();
            var referenceEntity = new GenericIdEntity<ComplexPrimaryKey>();

            var guidResult = DB.IsNew(guidEntity);
            var guidNullableResult = DB.IsNew(guidNullableEntity);
            var intResult = DB.IsNew(intEntity);
            var unsignedIntResult = DB.IsNew(unsignedIntEntity);
            var shortResult = DB.IsNew(shortEntity);
            var unsignedShortResult = DB.IsNew(unsignedShortEntity);
            var longResult = DB.IsNew(longEntity);
            var unsignedLongResult = DB.IsNew(unsignedLongEntity);
            var stringResult = DB.IsNew(stringEntity);
            var referenceResult = DB.IsNew(referenceEntity);

            guidResult.ShouldBeTrue();
            guidNullableResult.ShouldBeTrue();
            intResult.ShouldBeTrue();
            unsignedIntResult.ShouldBeTrue();
            shortResult.ShouldBeTrue();
            unsignedShortResult.ShouldBeTrue();
            longResult.ShouldBeTrue();
            unsignedLongResult.ShouldBeTrue();
            stringResult.ShouldBeTrue();
            referenceResult.ShouldBeTrue();
        }

        [Fact]
        public void IsNew_GivenNonTransientEntity_ShouldBeValid()
        {
            var guidEntity = new GenericIdEntity<Guid> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") };
            var guidNullableEntity = new GenericIdEntity<Guid?> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") };
            var intEntity = new GenericIdEntity<int> { Id = 1 };
            var unsignedIntEntity = new GenericIdEntity<uint> { Id = 1 };
            var shortEntity = new GenericIdEntity<short>() { Id = 1 };
            var unsignedShortEntity = new GenericIdEntity<ushort>() { Id = 1 };
            var longEntity = new GenericIdEntity<long>() { Id = 1 };
            var unsignedLongEntity = new GenericIdEntity<ulong>() { Id = 1 };
            var stringEntity = new GenericIdEntity<string>() { Id = "ID-1" };
            var referenceEntity = new GenericIdEntity<ComplexPrimaryKey> { Id = new ComplexPrimaryKey() };

            var guidResult = DB.IsNew(guidEntity);
            var guidNullableResult = DB.IsNew(guidNullableEntity);
            var intResult = DB.IsNew(intEntity);
            var unsignedIntResult = DB.IsNew(unsignedIntEntity);
            var shortResult = DB.IsNew(shortEntity);
            var unsignedShortResult = DB.IsNew(unsignedShortEntity);
            var longResult = DB.IsNew(longEntity);
            var unsignedLongResult = DB.IsNew(unsignedLongEntity);
            var stringResult = DB.IsNew(stringEntity);
            var referenceResult = DB.IsNew(referenceEntity);

            guidResult.ShouldBeFalse();
            guidNullableResult.ShouldBeFalse();
            intResult.ShouldBeFalse();
            unsignedIntResult.ShouldBeFalse();
            shortResult.ShouldBeFalse();
            unsignedShortResult.ShouldBeFalse();
            longResult.ShouldBeFalse();
            unsignedLongResult.ShouldBeFalse();
            stringResult.ShouldBeFalse();
            referenceResult.ShouldBeFalse();
        }

        [Fact]
        public void IsNew_GivenTransientEntityAndPrimaryKeyName_ShouldBeValid()
        {
            var guidEntity = new GenericNoMapsIdEntity<Guid>();
            var guidNullableEntity = new GenericNoMapsIdEntity<Guid?>();
            var intEntity = new GenericNoMapsIdEntity<int>();
            var unsignedIntEntity = new GenericNoMapsIdEntity<uint>();
            var shortEntity = new GenericNoMapsIdEntity<short>();
            var unsignedShortEntity = new GenericNoMapsIdEntity<ushort>();
            var longEntity = new GenericNoMapsIdEntity<long>();
            var unsignedLongEntity = new GenericNoMapsIdEntity<ulong>();
            var stringEntity = new GenericNoMapsIdEntity<string>();
            var referenceEntity = new GenericNoMapsIdEntity<ComplexPrimaryKey>();

            var guidResult = DB.IsNew("Id", guidEntity);
            var guidNullableResult = DB.IsNew("Id", guidNullableEntity);
            var intResult = DB.IsNew("Id", intEntity);
            var unsignedIntResult = DB.IsNew("Id", unsignedIntEntity);
            var shortResult = DB.IsNew("Id", shortEntity);
            var unsignedShortResult = DB.IsNew("Id", unsignedShortEntity);
            var longResult = DB.IsNew("Id", longEntity);
            var unsignedLongResult = DB.IsNew("Id", unsignedLongEntity);
            var stringResult = DB.IsNew("Id", stringEntity);
            var referenceResult = DB.IsNew("Id", referenceEntity);

            guidResult.ShouldBeTrue();
            guidNullableResult.ShouldBeTrue();
            intResult.ShouldBeTrue();
            unsignedIntResult.ShouldBeTrue();
            shortResult.ShouldBeTrue();
            unsignedShortResult.ShouldBeTrue();
            longResult.ShouldBeTrue();
            unsignedLongResult.ShouldBeTrue();
            stringResult.ShouldBeTrue();
            referenceResult.ShouldBeTrue();
        }

        [Fact]
        public void IsNew_GivenNonTransientEntityAndPrimaryKeyName_ShouldBeValid()
        {
            var guidEntity = new GenericNoMapsIdEntity<Guid> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") };
            var guidNullableEntity = new GenericNoMapsIdEntity<Guid?> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") };
            var intEntity = new GenericNoMapsIdEntity<int> { Id = 1 };
            var unsignedIntEntity = new GenericNoMapsIdEntity<uint> { Id = 1 };
            var shortEntity = new GenericNoMapsIdEntity<short>() { Id = 1 };
            var unsignedShortEntity = new GenericNoMapsIdEntity<ushort>() { Id = 1 };
            var longEntity = new GenericNoMapsIdEntity<long>() { Id = 1 };
            var unsignedLongEntity = new GenericNoMapsIdEntity<ulong>() { Id = 1 };
            var stringEntity = new GenericNoMapsIdEntity<string>() { Id = "ID-1" };
            var referenceEntity = new GenericNoMapsIdEntity<ComplexPrimaryKey> { Id = new ComplexPrimaryKey() };

            var guidResult = DB.IsNew("id", guidEntity);
            var guidNullableResult = DB.IsNew("id", guidNullableEntity);
            var intResult = DB.IsNew("id", intEntity);
            var unsignedIntResult = DB.IsNew("id", unsignedIntEntity);
            var shortResult = DB.IsNew("id", shortEntity);
            var unsignedShortResult = DB.IsNew("id", unsignedShortEntity);
            var longResult = DB.IsNew("id", longEntity);
            var unsignedLongResult = DB.IsNew("id", unsignedLongEntity);
            var stringResult = DB.IsNew("id", stringEntity);
            var referenceResult = DB.IsNew("id", referenceEntity);

            guidResult.ShouldBeFalse();
            guidNullableResult.ShouldBeFalse();
            intResult.ShouldBeFalse();
            unsignedIntResult.ShouldBeFalse();
            shortResult.ShouldBeFalse();
            unsignedShortResult.ShouldBeFalse();
            longResult.ShouldBeFalse();
            unsignedLongResult.ShouldBeFalse();
            stringResult.ShouldBeFalse();
            referenceResult.ShouldBeFalse();
        }

        [ExplicitColumns]
        [TableName("Orders")]
        [PrimaryKey("Id")]
        public class GenericIdEntity<T>
        {
            [Column]
            public T Id { get; set; }
        }

        public class GenericNoMapsIdEntity<T>
        {
            public T Id { get; set; }
        }

        public class ComplexPrimaryKey
        {
        }

        public class EntityWithoutConventionId
        {
            public int SomeOtherId { get; set; }

            public DateTime CreatedOn { get; set; }

            public string Text { get; set; }
        }
    }
}