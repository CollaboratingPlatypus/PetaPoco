using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PetaPoco.Core;
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
            ((IBuildConfigurationSettings) config).SetSetting("key", "value");

            string value = null;
            ((IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => value = v);
            value.ShouldNotBeNull();
            value.ShouldBe("value");
        }

        [Fact]
        public void SetSetting_GivenKeyAndNull_ShouldRemoveSetting()
        {
            ((IBuildConfigurationSettings) config).SetSetting("key", "value");
            ((IBuildConfigurationSettings) config).SetSetting("key", null);

            var getCalled = false;
            ((IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => { getCalled = true; });
            getCalled.ShouldBeFalse();
        }

        [Fact]
        public void TryGetSetting_GivenKeyAndValue_ShouldGetSetting()
        {
            ((IBuildConfigurationSettings) config).SetSetting("key", "value");

            string value = null;
            ((IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => value = v);
            value.ShouldNotBeNull();
            value.ShouldBe("value");
        }

        [Fact]
        public void TryGetSetting_GivenKeyThatDoesNotMatchValue_ShouldNotCallback()
        {
            var getCalled = false;
            ((IBuildConfigurationSettings) config).TryGetSetting<string>("key", v => { getCalled = true; });
            getCalled.ShouldBeFalse();
        }

        [Fact]
        public void TrySetSetting_GivenNullKey_Throws()
        {
            Should.Throw<ArgumentNullException>(() => ((IBuildConfigurationSettings) config).SetSetting(null, "value"));
        }

        [Fact]
        public void TryGetSetting_GivenNullKey_Throws()
        {
            Should.Throw<ArgumentNullException>(() => ((IBuildConfigurationSettings) config).TryGetSetting<string>(null, v => { }));
        }

        [Fact]
        public void TryGetSetting_GivenNullCallback_Throws()
        {
            ((IBuildConfigurationSettings) config).SetSetting("key", "value");
            Should.Throw<NullReferenceException>(() => ((IBuildConfigurationSettings) config).TryGetSetting<string>("key", null));
        }

        [Fact]
        public void UsingCreate_GivenMinimalConfiguration_ShouldNotAffectPetaPocoDefaults()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().Create();

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
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingCommandTimeout(50).Create();

            db.CommandTimeout.ShouldBe(50);
        }

        [Fact]
        public void WithNamedParams_AfterCreate_ShouldBeSetOnPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().WithNamedParams().Create();

            db.EnableNamedParams.ShouldBeTrue();
        }

        [Fact]
        public void WithoutNamedParams_AfterCreate_ShouldNotBeSetOnPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().WithoutNamedParams().Create();

            db.EnableNamedParams.ShouldBeFalse();
        }

        [Fact]
        public void WithAutoSelect_AfterCreate_ShouldBeSetOnPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().WithAutoSelect().Create();

            db.EnableNamedParams.ShouldBeTrue();
        }

        [Fact]
        public void WithoutAutoSelect_AfterCreate_ShouldNotBeSetOnPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().WithoutAutoSelect().Create();

            db.EnableAutoSelect.ShouldBeFalse();
        }

        [Fact]
        public void UsingConnectionString_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentException>(() => config.UsingConnectionString(null));
            Should.Throw<ArgumentException>(() => config.UsingConnectionString(string.Empty));
        }

        [Fact]
        public void UsingProvider_Overrides_UsingProviderName()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<FakeProvider>().UsingProviderName("OracleDatabaseProvider").Create();

            db.Provider.ShouldBeOfType<FakeProvider>();
        }

        [Fact]
        public void UsingConnectionString_BadProvider_Throws()
        {
            config.UsingConnectionString("cs").UsingProviderName("pn");
            Should.Throw<ArgumentException>(() => config.Create());
        }

        [Fact]
        public void UsingConnectionString_NoProvider_Throws()
        {
            Should.Throw<InvalidOperationException>(() => config.UsingConnectionString("cs").Create());
        }

        [Fact]
        public void UsingConnectionString_GivenTimeoutAndAfterCreate_ShouldBeSameAsPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().Create();

            db.ConnectionString.ShouldBe("cs");
        }

#if !NETCOREAPP
        [Fact]
        public void UsingConnectionStringName_GivenInvalidArguments_Throws()
        {
            Should.Throw<ArgumentException>(() => config.UsingConnectionStringName(null));
            Should.Throw<ArgumentException>(() => config.UsingConnectionStringName(string.Empty));
        }
#endif

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
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingDefaultMapper(new StandardMapper()).Create();

            var db1 = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingDefaultMapper<StandardMapper>().Create();

            db.DefaultMapper.ShouldBeOfType<StandardMapper>();
            db1.DefaultMapper.ShouldBeOfType<StandardMapper>();
        }

        [Fact]
        public void UsingDefaultMapper_GivenMapperOrTypeAndConfigurationCallback_ShouldBeSameAsPetaPocoInstanceAndCallback()
        {
            var dbCalled = false;
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingDefaultMapper(new StandardMapper(), sm => dbCalled = true).Create();

            var db1Called = false;
            var db1 = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingDefaultMapper<StandardMapper>(sm => db1Called = true).Create();

            dbCalled.ShouldBeTrue();
            db.DefaultMapper.ShouldBeOfType<StandardMapper>();
            db1Called.ShouldBeTrue();
            db1.DefaultMapper.ShouldBeOfType<StandardMapper>();
        }

        [Fact]
        public void UsingIsolationLevel_GivenIsolationLevelAndAfterCreate_ShouldBeSameAsPetaPocoInstance()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingIsolationLevel(IsolationLevel.Chaos).Create();

            db.IsolationLevel.ShouldBe(IsolationLevel.Chaos);
        }

        [Fact]
        public void NotUsingIsolationLevel_AfterCreate_PetaPocoInstanceShouldBeNull()
        {
            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().Create();

            db.IsolationLevel.ShouldBeNull();
        }

        [Fact]
        public void UsingCommandExecuting_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbCommandEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingCommandExecuting(handler).Create();

            // Can't inspect the event directly, so we have to get it to fire
            (db as Database).OnExecutingCommand(null);
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingCommandExecuted_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbCommandEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingCommandExecuted(handler).Create();

            (db as Database).OnExecutedCommand(null);
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingConnectionClosing_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbConnectionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingConnectionClosing(handler).Create();

            (db as Database).OnConnectionClosing(null);
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingConnectionOpening_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbConnectionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingConnectionOpening(handler).Create();

            (db as Database).OnConnectionOpening(null);
            eventFired.ShouldBeTrue();
        }
        
        [Fact]
        public void UsingConnectionOpened_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbConnectionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingConnectionOpened(handler).Create();

            (db as Database).OnConnectionOpened(null);
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingTransactionStarted_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbTransactionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingTransactionStarted(handler).Create();

            (db as Database).OnBeginTransaction();
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingTransactionEnding_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<DbTransactionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingTransactionEnding(handler).Create();

            (db as Database).OnEndTransaction();
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingExceptionThrown_AfterCreate_InstanceShouldHaveDelegate()
        {
            bool eventFired = false;
            EventHandler<ExceptionEventArgs> handler = (sender, args) => eventFired = true;

            var db = config.UsingConnectionString("cs").UsingProvider<SqlServerDatabaseProvider>().UsingExceptionThrown(handler).Create();

            (db as Database).OnException(new Exception());
            eventFired.ShouldBeTrue();
        }

        [Fact]
        public void UsingConnection_AfterCreate_InstanceShouldBeValid()
        {
            var connString = "Data Source = foo";
            var connection = new SqlConnection(connString);
            var db = new Database(config.UsingConnection(connection));

            db.ConnectionString.ShouldBe(connString);
            db.Provider.ShouldBeOfType<SqlServerDatabaseProvider>();
        }

        public class FakeProvider : DatabaseProvider
        {
            public override DbProviderFactory GetFactory()
                => null;
        }

        [Fact]
        public void UsingConnectionWithProvider_AfterCreate_InstanceShouldBeValid()
        {
            var connString = "Data Source = foo";
            var connection = new SqlConnection(connString);
            var db = new Database(config.UsingConnection(connection).UsingProvider<FakeProvider>());

            db.ConnectionString.ShouldBe(connString);
            db.Provider.ShouldBeOfType<FakeProvider>();
        }

        [Fact]
        public void UsingConnectionWithProviderName_AfterCreate_InstanceShouldBeValid()
        {
            DatabaseProvider.RegisterCustomProvider<FakeProvider>("fake");
            try
            {
                var connString = "Data Source = foo";
                var connection = new SqlConnection(connString);
                var db = new Database(config.UsingConnection(connection).UsingProviderName("FakeProvider"));

                db.ConnectionString.ShouldBe(connString);
                db.Provider.ShouldBeOfType<FakeProvider>();
            }
            finally
            {
                DatabaseProvider.ClearCustomProviders();
            }
        }
    }
}