// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class DBTestProvider : IDisposable
    {
        protected abstract IDatabase Database { get; }

        protected abstract string ScriptResourceName { get; }

        public virtual void Dispose()
        {
        }

        public virtual IDatabase Execute()
        {
            var db = Database;
            using (var s = this.GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
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

        protected virtual IDatabase LoadFromConnectionName(string name)
        {
#if NETCOREAPP
            var appSettings = AppSetting.Load();

            return DatabaseConfiguration.Build()
                .UsingConnectionString(appSettings.ConnectionStrings.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ConnectionString)
                .UsingProviderName(appSettings.ConnectionStrings.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ProviderName)
                .Create();            
#else
            return DatabaseConfiguration.Build().UsingConnectionStringName(name).Create();
#endif
        }
    }
}