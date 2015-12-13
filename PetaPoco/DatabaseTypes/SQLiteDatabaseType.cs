// <copyright file="SQLiteDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using PetaPoco.Core;
using PetaPoco.Internal;

namespace PetaPoco.DatabaseTypes
{
    public class SQLiteDatabaseType : DatabaseType
    {
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
                return db.ExecuteScalarHelper(cmd);
            }
            else
            {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }

        public override string GetExistsSql()
        {
            return "SELECT EXISTS (SELECT 1 FROM {0} WHERE {1})";
        }
    }
}