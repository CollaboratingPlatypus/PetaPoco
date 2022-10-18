using System;
using System.IO;
using System.Text;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class DBTestProvider : IDisposable
    {
        protected abstract string ConnectionName { get; }

        public string ProviderName => GetProviderName(ConnectionName);

        protected IDatabase Database => LoadFromConnectionName(ConnectionName);

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

        protected virtual void ExecuteBuildScript(IDatabase database, string script)
        {
            database.Execute(script);
        }

        protected virtual IDatabaseBuildConfiguration BuildFromConnectionName(string name)
        {
            return DatabaseConfiguration.Build()
                .UsingConnectionString(AppSetting.Instance.ConnectionStringFor(name).ConnectionString)
                .UsingProviderName(AppSetting.Instance.ConnectionStringFor(name).ProviderName);
        }

        protected virtual IDatabase LoadFromConnectionName(string name)
        {
            return BuildFromConnectionName(name).Create();
        }

        public string GetProviderName(string name)
        {
            return AppSetting.Instance.ConnectionStringFor(name).ProviderName;
        }

        public virtual void AddColumnToTable(string table, string column, string columnType)
        {
            Database.Execute($"ALTER TABLE {Database.Provider.EscapeSqlIdentifier(table)} ADD {Database.Provider.EscapeSqlIdentifier(column)} {columnType} NULL");
        }

        public virtual void DropColumnFromTable(string table, string column)
        {
            Database.Execute($"ALTER TABLE {Database.Provider.EscapeSqlIdentifier(table)} DROP COLUMN {Database.Provider.EscapeSqlIdentifier(column)}");
        }
    }
}
