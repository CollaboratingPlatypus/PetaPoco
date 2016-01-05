// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/28</date>

using System;
using PetaPoco.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class DatabaseConfigurationTests
    {
        private readonly IDatabaseBuildConfiguration config;

        public DatabaseConfigurationTests()
        {
            config = DatabaseConfiguration.Build();
        }

        [Fact]
        public void SetSetting_GivenKeyAndValue_ShouldSetSetting()
        {
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting("key", "value");

            string value = null;
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => value = v);
            value.ShouldNotBeNull();
            value.ShouldBe("value");
        }

        [Fact]
        public void SetSetting_GivenKeyAndNull_ShouldRemoveSetting()
        {
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting("key", "value");
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting("key", null);

            var getCalled = false;
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => { getCalled = true; });
            getCalled.ShouldBeFalse();
        }

        [Fact]
        public void TryGetSetting_GivenKeyAndValue_ShouldGetSetting()
        {
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting("key", "value");

            string value = null;
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => value = v);
            value.ShouldNotBeNull();
            value.ShouldBe("value");
        }

        [Fact]
        public void TryGetSetting_GivenKeyThatDoesNotMatchValue_ShouldNotCallback()
        {
            var getCalled = false;
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => { getCalled = true; });
            getCalled.ShouldBeFalse();
        }

        [Fact]
        public void TrySetSetting_GivenNullKey_Throws()
        {
            Should.Throw<ArgumentNullException>(() => ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting(null, "value"));
        }

        [Fact]
        public void TryGetSetting_GivenNullKey_Throws()
        {
            Should.Throw<ArgumentNullException>(() => ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>(null, v => { }));
        }

        [Fact]
        public void TryGetSetting_GivenNullCallback_Throws()
        {
            ((DatabaseConfiguration.IBuildConfigurationSettings) config).SetSetting("key", "value");
            Should.Throw<NullReferenceException>(() => ((DatabaseConfiguration.IBuildConfigurationSettings) config).TryGetSetting<string>("key", null));
        }

        [Fact]
        public void UsingCreate_GivenMinimalConfiguration_ShouldNotAffectPetaPoocDefaults()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .Create();

            db.CommandTimeout.ShouldBe(0);
            db.Provider.ShouldBeOfType<SqlServerDatabaseProvider>();
            db.ConnectionString.ShouldBe("cs");
            db.DefaultMapper.ShouldBeOfType<ConventionMapper>();
            db.EnableAutoSelect.ShouldBeTrue();
            db.EnableNamedParams.ShouldBeTrue();
        }

        [Fact]
        public void UsingCommandTimeout_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentException>(() => config.UsingCommandTimeout(0));
            Should.Throw<ArgumentException>(() => config.UsingCommandTimeout(-1));
        }

        [Fact]
        public void UsingCommandTimeout_GivenTimeoutAndAfterCreate_ShouldBeSameAsPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .UsingCommandTimeout(50)
                .Create();

            db.CommandTimeout.ShouldBe(50);
        }

        [Fact]
        public void WithNamedParams_AfterCreate_ShouldBeSetOnPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .WithNamedParams()
                .Create();

            db.EnableNamedParams.ShouldBeTrue();
        }

        [Fact]
        public void WithoutNamedParams_AfterCreate_ShouldNotBeSetOnPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .WithoutNamedParams()
                .Create();

            db.EnableNamedParams.ShouldBeFalse();
        }

        [Fact]
        public void WithAutoSelect_AfterCreate_ShouldBeSetOnPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .WithAutoSelect()
                .Create();

            db.EnableNamedParams.ShouldBeTrue();
        }

        [Fact]
        public void WithoutAutoSelect_AfterCreate_ShouldNotBeSetOnPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .WithoutAutoSelect()
                .Create();

            db.EnableAutoSelect.ShouldBeFalse();
        }

        [Fact]
        public void UsingConnectionString_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentException>(() => config.UsingConnectionString(null));
            Should.Throw<ArgumentException>(() => config.UsingConnectionString(string.Empty));
        }

        [Fact]
        public void UsingConnectionString_GivenTimeoutAndAfterCreate_ShouldBeSameAsPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .Create();

            db.ConnectionString.ShouldBe("cs");
        }

        [Fact]
        public void UsingConnectionStringName_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentException>(() => config.UsingConnectionStringName(null));
            Should.Throw<ArgumentException>(() => config.UsingConnectionStringName(string.Empty));
        }

        [Fact(Skip = "Can't be tested as testing would require connection strings in the app/web config.")]
        public void UsingConnectionStringName_GivenTimeoutAndAfterCreate_ShouldBeSameAsPetaPocoInstance()
        {
        }

        [Fact]
        public void UsingDefaultMapper_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentNullException>(() => config.UsingDefaultMapper((StandardMapper) null));
            Should.Throw<ArgumentNullException>(() => config.UsingDefaultMapper((StandardMapper) null, null));
            Should.Throw<ArgumentNullException>(() => config.UsingDefaultMapper(new ConventionMapper(), null));
            Should.Throw<ArgumentNullException>(() => config.UsingDefaultMapper((Action<StandardMapper>) null));
        }

        [Fact]
        public void UsingDefaultMapper_GivenMapperOrType_ShouldBeSameAsPetaPocoInstance()
        {
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .UsingDefaultMapper(new StandardMapper())
                .Create();

            var db1 = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .UsingDefaultMapper<StandardMapper>()
                .Create();

            db.DefaultMapper.ShouldBeOfType<StandardMapper>();
            db1.DefaultMapper.ShouldBeOfType<StandardMapper>();
        }

        [Fact]
        public void UsingDefaultMapper_GivenMapperOrTypeAndConfigurationCallback_ShouldBeSameAsPetaPocoInstanceAndCallback()
        {
            var dbCalled = false;
            var db = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .UsingDefaultMapper(new StandardMapper(), sm => dbCalled = true)
                .Create();

            var db1Called = false;
            var db1 = config
                .UsingConnectionString("cs")
                .UsingProvider<SqlServerDatabaseProvider>()
                .UsingDefaultMapper<StandardMapper>(sm => db1Called = true)
                .Create();

            dbCalled.ShouldBeTrue();
            db.DefaultMapper.ShouldBeOfType<StandardMapper>();
            db1Called.ShouldBeTrue();
            db1.DefaultMapper.ShouldBeOfType<StandardMapper>();
        }
    }
}