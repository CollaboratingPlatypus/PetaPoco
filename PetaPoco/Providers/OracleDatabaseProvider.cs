// <copyright file="OracleDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Data;
using System.Data.Common;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    public class OracleDatabaseProvider : DatabaseProvider
    {
        public override string GetParameterPrefix(string connectionString)
        {
            return ":";
        }

        public override void PreExecute(IDbCommand cmd)
        {
            cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
            cmd.GetType().GetProperty("InitialLONGFetchSize").SetValue(cmd, -1, null);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (parts.SqlSelectRemoved.StartsWith("*"))
                throw new Exception("Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");

            // Same deal as SQL Server
            return Singleton<SqlServerDatabaseProvider>.Instance.BuildPageQuery(skip, take, parts, ref args);
        }

        public override DbProviderFactory GetFactory()
        {
            // "Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess" is for Oracle.ManagedDataAccess.dll
            // "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess" is for Oracle.DataAccess.dll
            return GetFactory("Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess, Culture=neutral, PublicKeyToken=89b483f429c47342",
                              "Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess");
        }

        public override string EscapeSqlIdentifier(string sqlIdentifier)
        {
            return string.Format("\"{0}\"", sqlIdentifier.ToUpperInvariant());
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
                ExecuteNonQueryHelper(db, cmd);
                return param.Value;
            }
            else
            {
                ExecuteNonQueryHelper(db, cmd);
                return -1;
            }
        }
    }
}