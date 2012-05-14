using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PetaPoco.DatabaseTypes;
using PetaPoco.Internal;

namespace PetaPoco.Internal
{
	abstract class DatabaseType
	{
		public virtual string GetParameterPrefix(string ConnectionString)
		{
			return "@";
		}

		public virtual object MapParameterValue(object value)
		{
			// Cast bools to integer
			if (value.GetType() == typeof(bool))
			{
				return ((bool)value) ? 1 : 0;
			}
	
			// Leave it
			return value;
		}

		public virtual void PreExecute(IDbCommand cmd)
		{
		}

		public virtual string BuildPageQuery(long skip, long take, PagingHelper.SQLParts parts, ref object[] args)
		{
			var sql = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", parts.sql, args.Length, args.Length + 1);
			args = args.Concat(new object[] { take, skip }).ToArray();
			return sql;
		}

		public virtual string GetExistsSql()
		{
			return "SELECT COUNT(*) FROM {0} WHERE {1}";
		}

		public virtual string EscapeTableName(string tableName)
		{
			// Assume table names with "dot" are already escaped
			return tableName.IndexOf('.') >= 0 ? tableName : EscapeSqlIdentifier(tableName);
		}

		public virtual string EscapeSqlIdentifier(string str)
		{
			return string.Format("[{0}]", str);
		}

		public virtual string GetAutoIncrementExpression(TableInfo ti)
		{
			return null;
		}

		public virtual string GetInsertOutputClause(string primaryKeyName)
		{
			return string.Empty;
		}

		public virtual object ExecuteInsert(Database db, IDbCommand cmd, string PrimaryKeyName)
		{
			cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
			return db.ExecuteScalarHelper(cmd);
		}


		public static DatabaseType Resolve(string TypeName, string ProviderName)
		{
			// Try using type name first (more reliable)
			if (TypeName.StartsWith("MySql")) 
				return Singleton<MySqlDatabaseType>.Instance;
			if (TypeName.StartsWith("SqlCe")) 
				return Singleton<SqlServerCEDatabaseType>.Instance;
			if (TypeName.StartsWith("Npgsql") || TypeName.StartsWith("PgSql")) 
				return Singleton<PostgreSQLDatabaseType>.Instance;
			if (TypeName.StartsWith("Oracle")) 
				return Singleton<OracleDatabaseType>.Instance;
			if (TypeName.StartsWith("SQLite")) 
				return Singleton<SQLiteDatabaseType>.Instance;
			if (TypeName.StartsWith("System.Data.SqlClient.")) 
				return Singleton<SqlServerDatabaseType>.Instance;
			
			// Try again with provider name
			if (ProviderName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) 
				return Singleton<MySqlDatabaseType>.Instance;
			if (ProviderName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0) 
				return Singleton<SqlServerCEDatabaseType>.Instance;
			if (ProviderName.IndexOf("pgsql", StringComparison.InvariantCultureIgnoreCase) >= 0) 
				return Singleton<PostgreSQLDatabaseType>.Instance;
			if (ProviderName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) 
				return Singleton<OracleDatabaseType>.Instance;
			if (ProviderName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0) 
				return Singleton<SQLiteDatabaseType>.Instance;

			// Assume SQL Server
			return Singleton<SqlServerDatabaseType>.Instance;
		}

	}
}
