// <copyright file="OracleDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Data;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;

namespace PetaPoco.DatabaseTypes
{
    public class OracleDatabaseType : DatabaseType
    {
        public override string GetParameterPrefix(string connectionString)
        {
            return ":";
        }

        public override void PreExecute(IDbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.SqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            // Same deal as SQL Server
            return Singleton<SqlServerDatabaseType>.Instance.BuildPageQuery(skip, take, parts, ref args);
        }

        public override string EscapeSqlIdentifier(string str)
        {
            return string.Format("\"{0}\"", str.ToUpperInvariant());
        }

        public override string GetAutoIncrementExpression(TableInfo ti)
        {
            if (!string.IsNullOrEmpty(ti.SequenceName))
                return string.Format("{0}.nextval", ti.SequenceName);

            return null;
        }

        public override object ExecuteInsert(Database db, IDbCommand cmd, string primaryKeyName)
        {
            if (primaryKeyName != null)
            {
                cmd.CommandText += string.Format(" returning {0} into :newid", EscapeSqlIdentifier(primaryKeyName));
                var param = cmd.CreateParameter();
                param.ParameterName = ":newid";
                param.Value = DBNull.Value;
                param.Direction = ParameterDirection.ReturnValue;
                param.DbType = DbType.Int64;
                cmd.Parameters.Add(param);
                db.ExecuteNonQueryHelper(cmd);
                return param.Value;
            }
            else
            {
                db.ExecuteNonQueryHelper(cmd);
                return -1;
            }
        }
    }
}