using System.Data.Common;
using PetaPoco.Core;

namespace PetaPoco.Providers
{
    public class SQLiteDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory("System.Data.SQLite.SQLiteFactory, System.Data.SQLite", "Microsoft.Data.Sqlite.SqliteFactory, Microsoft.Data.Sqlite");
        }

        public override object MapParameterValue(object value)
        {
            if (value.GetType() == typeof(uint))
                return (long) ((uint) value);

            return base.MapParameterValue(value);
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += ";\nSELECT last_insert_rowid();";
                return ExecuteScalarHelper(db, cmd);
            }
            else
            {
                ExecuteNonQueryHelper(db, cmd);
                return -1;
            }
        }

        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }
    }
}