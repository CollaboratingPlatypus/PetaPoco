// <copyright file="PostgreSQLDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using PetaPoco.Internal;

namespace PetaPoco.DatabaseTypes
{
    internal class PostgreSQLDatabaseType : DatabaseType
    {
        public override bool HasNativeGuidSupport
        {
            get { return true; }
        }

        public override object MapParameterValue(object value)
        {
            // Don't map bools to ints in PostgreSQL
            if (value.GetType() == typeof(bool))
                return value;

            return base.MapParameterValue(value);
        }

        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("\"{0}\"", str);
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string PrimaryKeyName)
        {
            if (PrimaryKeyName != null)
            {
                cmd.CommandText += string.Format("returning {0} as NewID", EscapeSqlIdentifier(PrimaryKeyName));
                return db.ExecuteScalarHelper(cmd);
            }
            else
            {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }
    }
}