// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using Moq;
using PetaPoco.Core;
using PetaPoco.Providers;
using PetaPoco.Tests.Unit.Models;
using PetaPoco.Utilities;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class DatabaseTests
    {
        private readonly DbProviderFactory _dbProviderFactory = new Mock<DbProviderFactory>(MockBehavior.Strict).Object;

        private readonly IProvider _provider = new Mock<IProvider>(MockBehavior.Loose).Object;

        private IDatabase DB { get; }

        public DatabaseTests()
        {
            DB = new Database("cs", _provider);
        }

        [Fact]
        public void Construct_GivenWrappedFactory_ShouldNotThrow()
        {
            new Database("some connection string", new DbProviderWrapper());
        }

        [Fact]
        public void Construct_GivenUnwrappedFactory_ShouldNotThrow()
        {
            new Database("some connection string", SqlClientFactory.Instance);
        }

        [Fact]
        public void Construct_GivenInvalidArguments_ShouldThrow()
        {
#if !NETCOREAPP
            Should.Throw<InvalidOperationException>(() => new Database());
#endif

            Should.Throw<ArgumentNullException>(() => new Database((IDbConnection) null));

            Should.Throw<ArgumentNullException>(() => new Database("some connection string", (IProvider) null));
            Should.Throw<ArgumentException>(() => new Database(null, _provider));

#if !NETCOREAPP
            Should.Throw<ArgumentException>(() => new Database((string) null));
            Should.Throw<InvalidOperationException>(() => new Database("some connection string"));
#endif
            Should.Throw<ArgumentException>(() => new Database("connection string", (string)null));
            Should.Throw<ArgumentException>(() => new Database("connection string", ""));
            Should.Throw<ArgumentException>(() => new Database(null, "provider name"));
            Should.Throw<ArgumentException>(() => new Database("", "provider name"));
            Should.Throw<ArgumentException>(() => new Database("connection string", "provider name"));

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
#if !NETCOREAPP
                    e.Message.ShouldContain("One or more connection strings");
#else
                    e.Message.ShouldContain("A connection string is required");
#endif
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
#if !NETCOREAPP
                    e.Message.ShouldContain("Both a connection string and provider are required");
#else
                    e.Message.ShouldContain("Either a provider name or provider must be registered");
#endif
                    throw;
                }
            });
            Should.Throw<ArgumentException>(() => new Database<SqlServerDatabaseProvider>(null));
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
        public void IsNew_GivenTransientEntity_ShoudlBeTrue()
        {
            DB.IsNew(new GenericIdEntity<Guid>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<Guid?>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<int>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<uint>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<short>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<ushort>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<long>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<ulong>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<string>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<ComplexPrimaryKey>()).ShouldBeTrue();
            DB.IsNew(new GenericIdEntity<decimal>()).ShouldBeTrue();
        }

        [Fact]
        public void IsNew_GivenNonTransientEntity_ShouldBeFalse()
        {
            DB.IsNew(new GenericIdEntity<Guid> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<Guid?> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<int> { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<uint> { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<short>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<ushort>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<long>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<ulong>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<string>() { Id = "ID-1" }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<ComplexPrimaryKey> { Id = new ComplexPrimaryKey() }).ShouldBeFalse();
            DB.IsNew(new GenericIdEntity<decimal>() { Id = 1.0M }).ShouldBeFalse();
        }

        [Fact]
        public void IsNew_GivenTransientEntityAndPrimaryKeyName_ShouldBeTrue()
        {
            DB.IsNew("Id", new GenericNoMapsIdEntity<Guid>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<Guid?>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<int>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<uint>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<short>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<ushort>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<long>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<ulong>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<string>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<ComplexPrimaryKey>()).ShouldBeTrue();
            DB.IsNew("Id", new GenericNoMapsIdEntity<decimal>()).ShouldBeTrue();
        }

        [Fact]
        public void IsNew_GivenNonTransientEntityAndPrimaryKeyName_ShouldBeFalse()
        {
            DB.IsNew("id", new GenericNoMapsIdEntity<Guid> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<Guid?> { Id = Guid.Parse("803A25C4-65D9-4F92-9305-0854FD134841") }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<int> { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<uint> { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<short>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<ushort>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<long>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<ulong>() { Id = 1 }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<string>() { Id = "ID-1" }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<ComplexPrimaryKey> { Id = new ComplexPrimaryKey() }).ShouldBeFalse();
            DB.IsNew("id", new GenericNoMapsIdEntity<decimal>() { Id = 1.0M }).ShouldBeFalse();
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

        public class DbProviderWrapper : DbProviderFactory, IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(DbProviderWrapper))
                    return SqlClientFactory.Instance;

                return null;
            }
        }        
    }
}