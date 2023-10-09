using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace PetaPoco.Tests.Integration
{
    public abstract class TestProvider : IDisposable
    {
        public string ProviderName => GetProviderName(ConnectionName);

        protected IDatabase Database => LoadFromConnectionName(ConnectionName);

        protected abstract string ConnectionName { get; }

        protected abstract string ScriptResourceName { get; }

        public virtual void Dispose()
        {
        }

        public virtual IDatabase Execute()
        {
            var db = Database;
            using (var s = GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            {
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    ExecuteBuildScript(db, r.ReadToEnd());
                }
            }

            return db;
        }

        public virtual void ExecuteBuildScript(IDatabase database, string script)
        {
            database.Execute(script);
        }

        protected virtual IDatabase LoadFromConnectionName(string connectionName)
        {
            return BuildFromConnectionName(connectionName).Create();
        }

        protected virtual IDatabaseBuildConfiguration BuildFromConnectionName(string connectionName)
        {
#if NETCOREAPP
            return DatabaseConfiguration.Build()
                .UsingConnectionString(AppSetting.Instance.ConnectionStringFor(connectionName).ConnectionString)
                .UsingProviderName(AppSetting.Instance.ConnectionStringFor(connectionName).ProviderName);
#else
            return DatabaseConfiguration.Build().UsingConnectionStringName(connectionName);
#endif
        }

        protected string GetProviderName(string connectionName)
        {
#if NETCOREAPP
            return AppSetting.Instance.ConnectionStringFor(connectionName).ProviderName;
#else
            return ConfigurationManager.ConnectionStrings[connectionName].ProviderName;
#endif
        }
    }
}
