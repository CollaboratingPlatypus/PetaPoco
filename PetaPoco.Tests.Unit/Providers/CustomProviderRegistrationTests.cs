using System;
using System.Data.Common;
using PetaPoco.Core;
using PetaPoco.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Providers
{
    public class CustomProviderRegistrationTests : IDisposable
    {
        public void Dispose()
        {
            DatabaseProvider.ClearCustomProviders();
        }

        private void RegisterProviders()
        {
            DatabaseProvider.RegisterCustomProvider<MyCustomProvider>("Foo");
            DatabaseProvider.RegisterCustomProvider<MyCustomProvider>("Bar");
        }

        [Theory]
        [InlineData("Foo", typeof(MyCustomProvider))]
        [InlineData("foo", typeof(MyCustomProvider))]
        [InlineData("FOO", typeof(MyCustomProvider))]
        [InlineData("Bar", typeof(MyCustomProvider))]
        [InlineData("Baz", typeof(SqlServerDatabaseProvider))]
        [InlineData("MySql.SomethingOrOther", typeof(MySqlDatabaseProvider))]
        public void Resolve_WithName_ShouldHaveExpectedProvider(string name, Type type)
        {
            RegisterProviders();
            var provider = DatabaseProvider.Resolve(name, true, "Data Source=foo");
            provider.GetType().ShouldBe(type);
        }

        [Theory]
        [InlineData(typeof(FooType), typeof(MyCustomProvider))]
        [InlineData(typeof(BarType), typeof(MyCustomProvider))]
        [InlineData(typeof(String), typeof(SqlServerDatabaseProvider))]
        [InlineData(typeof(MariaDbType), typeof(MariaDbDatabaseProvider))]
        public void Resolve_WithType_ShouldHaveExpectedProvider(Type inputType, Type providerType)
        {
            RegisterProviders();
            var provider = DatabaseProvider.Resolve(inputType, true, "Data Source=foo");
            provider.GetType().ShouldBe(providerType);
        }

        [Fact]
        public void ChangingRegistration_ShouldHaveExpectedProvider()
        {
            RegisterProviders();
            DatabaseProvider.RegisterCustomProvider<MyOtherCustomProvider>("Foo");
            var provider = DatabaseProvider.Resolve("foo", true, "Data Source=foo");
            provider.GetType().ShouldBe(typeof(MyOtherCustomProvider));
        }

        [Fact]
        public void NoCustomRegistrations_ShouldHaveDefaultProvider()
        {
            var provider = DatabaseProvider.Resolve("foo", true, "Data Source=foo");
            provider.GetType().ShouldBe(typeof(SqlServerDatabaseProvider));
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void InvalidString_ShouldThrow(string input)
        {
            Should.Throw<ArgumentException>(() => DatabaseProvider.RegisterCustomProvider<MyCustomProvider>(input));
        }

        private class MyCustomProvider : DatabaseProvider
        {
            public override DbProviderFactory GetFactory()
            {
                throw new NotImplementedException();
            }
        }

        private class MyOtherCustomProvider : DatabaseProvider
        {
            public override DbProviderFactory GetFactory()
            {
                throw new NotImplementedException();
            }
        }

        private class FooType
        {
        }

        private class BarType
        {
        }

        private class MariaDbType
        {
        }
    }
}