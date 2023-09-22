using System;
using System.Configuration;
using System.IO;
using System.Text;

#if NETCOREAPP
using System.Linq;
#endif

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class DBTestProvider : IDisposable
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

        protected virtual IDatabaseBuildConfiguration BuildFromConnectionName(string name)
        {
#if NETCOREAPP
            return DatabaseConfiguration.Build()
                .UsingConnectionString(AppSetting.Instance.ConnectionStringFor(name).ConnectionString)
                .UsingProviderName(AppSetting.Instance.ConnectionStringFor(name).ProviderName);
#else
            return DatabaseConfiguration.Build().UsingConnectionStringName(name);
#endif
        }

        protected virtual IDatabase LoadFromConnectionName(string name)
        {
            return BuildFromConnectionName(name).Create();
        }

        public string GetProviderName(string name)
        {
#if NETCOREAPP
            return AppSetting.Instance.ConnectionStringFor(name).ProviderName;
#else
            return ConfigurationManager.ConnectionStrings[name].ProviderName;
#endif
        }
    }
}
