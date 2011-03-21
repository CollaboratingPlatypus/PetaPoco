using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;

namespace PetaPoco
{
	// Poco's marked [Explicit] require all column properties to be marked
	[AttributeUsage(AttributeTargets.Class)]
	public class ExplicitColumns : Attribute
	{
	}
	// For non-explicit pocos, causes a property to be ignored
	[AttributeUsage(AttributeTargets.Property)]
	public class Ignore : Attribute
	{
	}

	// For explicit pocos, marks property as a column
	[AttributeUsage(AttributeTargets.Property)]
	public class Column : Attribute
	{
	}

	// Specify the table name of a poco
	[AttributeUsage(AttributeTargets.Class)]
	public class TableName : Attribute
	{
		public TableName(string tableName)
		{
			Value = tableName;
		}
		public string Value { get; private set; }
	}

	// Specific the primary key of a poco class
	[AttributeUsage(AttributeTargets.Class)]
	public class PrimaryKey : Attribute
	{
		public PrimaryKey(string primaryKey)
		{
			Value = primaryKey;
		}

		public string Value { get; private set; }
	}

	// PocoData stores details on how to bind a poco to table
	public class PocoData
	{
		static Dictionary<Type, PocoData> m_PocoData = new Dictionary<Type, PocoData>();
		public static PocoData ForType(Type t)
		{
			lock (m_PocoData)
			{
				PocoData pd;
				if (!m_PocoData.TryGetValue(t, out pd))
				{
					pd = new PocoData(t);
					m_PocoData.Add(t, pd);
				}
				return pd;
			}
		}

		public PocoData(Type t)
		{
			// Get the table name
			var a = t.GetCustomAttributes(typeof(TableName), true);
			TableName = a.Length == 0 ? t.Name : (a[0] as TableName).Value;

			// Get the primary key
			a = t.GetCustomAttributes(typeof(PrimaryKey), true);
			PrimaryKey = a.Length == 0 ? "ID" : (a[0] as PrimaryKey).Value;

			// Work out bound properties
			bool ExplicitColumns = t.GetCustomAttributes(typeof(ExplicitColumns), true).Length > 0;
			Columns = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (var pi in t.GetProperties())
			{
				if (pi.GetCustomAttributes(ExplicitColumns ? typeof(Column) : typeof(Ignore), true).Length == 0)
					continue;

				Columns.Add(pi.Name, pi);
			}
		}

		public string TableName { get; private set; }
		public string PrimaryKey { get; private set; }
		public Dictionary<string, PropertyInfo> Columns { get; private set; }
	}

	// Miscellaneous extension methods
	public static class Extensions
	{
		// Add a parameter to a DB command
		public static void AddParam(this DbCommand cmd, object item, string ParameterPrefix)
		{
			var p = cmd.CreateParameter();
			p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
			if (item == null)
			{
				p.Value = DBNull.Value;
			}
			else
			{
				if (item.GetType() == typeof(Guid))
				{
					p.Value = item.ToString();
					p.DbType = DbType.String;
					p.Size = 4000;
				}
				else if (item.GetType() == typeof(string))
				{
					p.Size = (item as string).Length + 1;
					p.Value = item;
				}
				else
				{
					p.Value = item;
				}
			}

			cmd.Parameters.Add(p);
		}
	}


	// ShareableConnection represents either a shared connection used by a transaction,
	// or a one-off connection if not in a transaction.
	// Non-shared connections are disposed 
	public class ShareableConnection : IDisposable
	{
		public ShareableConnection(DbConnection connection, bool shared)
		{
			_connection = connection;
			_shared = shared;
		}

		public DbConnection Connection
		{
			get
			{
				return _connection;	
			}
		}

		DbConnection _connection;
		bool _shared;

		public void Dispose()
		{
			if (!_shared)
			{
				_connection.Dispose();
			}
		}
	}

	// Database class ... this is where most of the action happens
	public class Database
	{
		// Constructor
		public Database(string connectionStringName)
		{
			// Get connection string name
            if (connectionStringName == "")
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;

			// Work out connection string and provider name
            var _providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null) 
			{
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                    _providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            } 
			else 
			{
                throw new InvalidOperationException("Can't find a connection string with the name '" + connectionStringName + "'");
            }

			// Store factory and connection string
            _factory = DbProviderFactories.GetFactory(_providerName);
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
			_transactionDepth = 0;

			if (_connectionString.IndexOf("Allow User Variables=true") >= 0 &&
					string.Compare(_providerName, "MySql.Data.MySqlClient", true) == 0)
			{
				_paramPrefix = "?";
			}
		}

		// Get the connection for this database
		public DbConnection OpenConnection()
		{
			var c = _factory.CreateConnection();
			c.ConnectionString = _connectionString;
			c.Open();
			return c;
		}

		// Get a shared connection to be used when in a transaction
		public ShareableConnection OpenSharedConnection()
		{
			if (_sharedConnection!=null)
				return new ShareableConnection(_sharedConnection, true);
			else
				return new ShareableConnection(OpenConnection(), false);
		}

		// Helper to create a transaction scope
		public Transaction Transaction
		{
			get
			{
				return new Transaction(this);
			}
		}

		// Use by derived repo generated by T4 templates
		public virtual void OnBeginTransaction() { }
		public virtual void OnEndTransaction() { }

		// Start a new transaction, can be nested, every call must be
		//	matched by a call to AbortTransaction or CompleteTransaction
		// Use `using (var scope=db.Transaction) { scope.Complete(); }` to ensure correct semantics
		public void BeginTransaction()
		{
			_transactionDepth++;

			if (_transaction == null)
			{
				_sharedConnection = OpenConnection();
				_transaction = _sharedConnection.BeginTransaction();
				OnBeginTransaction();
			}

		}

		// Internal helper to cleanup transaction stuff
		void CleanupTransaction()
		{
			OnEndTransaction();

			_transaction.Dispose();
			_transaction = null;

			// Clean up connection
			_sharedConnection.Close();
			_sharedConnection.Dispose();
			_sharedConnection = null;
		}

		// Abort the entire outer most transaction scope
		public void AbortTransaction()
		{
			_transactionDepth--;

			if (_transaction != null)
			{
				// Rollback transaction
				_transaction.Rollback();
				CleanupTransaction();
			}
		}

		// Complete the transaction
		// To actually complete the whole transaction, every BeginTransaction must be matched
		// by a CompleteTransaction.
		public void CompleteTransaction()
		{
			_transactionDepth--;

			if (_transactionDepth==0 && _transaction != null)
			{
				// Commit transaction
				_transaction.Commit();
				CleanupTransaction();
			}

		}


		// Create a command
		public DbCommand CreateCommand(DbConnection connection, string sql, params object[] args)
		{
			// If we're in MySQL "Allow User Variables", we need to fix up parameter prefixes
			if (_paramPrefix == "?")
			{
				// Convert "@parameter" -> "?parameter"
				Regex paramReg = new Regex(@"(?<!@)@\w+");
				sql = paramReg.Replace(sql, m => "?" + m.Value.Substring(1));

				// Convert @@uservar -> @uservar and @@@systemvar -> @@systemvar
				sql = sql.Replace("@@", "@");
			}

			// Save the last sql and args
			_lastSql = sql;
			_lastArgs = args;

			DbCommand result = null;
			result = _factory.CreateCommand();
			result.Connection = connection;
			result.CommandText = sql;
			if (args.Length > 0)
			{
				foreach (var item in args)
				{
					result.AddParam(item, _paramPrefix);
				}
			}
			return result;
		}

		// Create a command
		public DbCommand CreateCommand(ShareableConnection connection, string sql, params object[] args)
		{
			return CreateCommand(connection.Connection, sql, args);
		}

		// Create a poco object for the current record in a data reader
		public T CreatePoco<T>(IDataReader r) where T:new()
		{
			var record = new T();
			var pd = PocoData.ForType(typeof(T));

			for (var i = 0; i < r.FieldCount; i++)
			{
				// Get property info
				var name=r.GetName(i);
				PropertyInfo pi;
				if (!pd.Columns.TryGetValue(name, out pi))
					continue;

				object val=r[i];

				if (pi != null)
				{
					if ((!pi.PropertyType.IsValueType || Nullable.GetUnderlyingType(pi.PropertyType) != null) 
							&& val != null 
							&& val.GetType() == typeof(DBNull))
					{
						val=null;
					}

					try
					{
						pi.SetValue(record, val, null);
					}
					catch (Exception x)
					{
						throw new Exception(string.Format("Failed to set property '{0}' on object of type '{1}' - {2}",
																	pi.Name, typeof(T).Name, x.Message));
					}
				}
			}

			return record;			
		}

		public virtual void OnException(Exception x)
		{
			System.Diagnostics.Debug.WriteLine(x.ToString());
			System.Diagnostics.Debug.WriteLine(LastCommand);
		}

		// Execute a non-query command
		public void Execute(string sql, params object[] args)
		{
			try
			{
				using (var conn = OpenSharedConnection())
				{
					using (var cmd = CreateCommand(conn, sql, args))
					{
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception x)
			{
				OnException(x);
				throw;
			}
		}

		string _paramPrefix = "@";

		public void Execute(Sql sql)
		{
			Execute(sql.SQL, sql.Arguments);
		}

		// Execute and cast a scalar property
		public T ExecuteScalar<T>(string sql, params object[] args)
		{
			try
			{
				using (var conn = OpenSharedConnection())
				{
					using (var cmd = CreateCommand(conn, sql, args))
					{
						object val = cmd.ExecuteScalar();
						return (T)val;
					}
				}
			}
			catch (Exception x)
			{
				OnException(x);
				throw;
			}
		}

		public T ExecuteScalar<T>(Sql sql)
		{
			return ExecuteScalar<T>(sql.SQL, sql.Arguments);
		}

		string AddSelectClause<T>(string sql)
		{
			// Already present?
			if (sql.Trim().StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
				return sql;

			// Get the poco data for this type
			var pd = new PocoData(typeof(T));
			return string.Format("SELECT * FROM {0} {1}", pd.TableName, sql);
		}

		// Return a typed list of pocos
        public List<T> Fetch<T>(string sql, params object[] args) where T:new()
		{
			try
			{
				using (var conn = OpenSharedConnection())
				{
					using (var cmd = CreateCommand(conn, AddSelectClause<T>(sql), args))
					{
						var r = cmd.ExecuteReader();
						var l = new List<T>();
						while (r.Read())
						{
							l.Add(CreatePoco<T>(r));
						}
						return l;
					}
				}
			}
			catch (Exception x)
			{
				OnException(x);
				throw;
			}
		}

		// Return an enumerable collection of pocos
		public IEnumerable<T> Query<T>(string sql, params object[] args) where T:new()
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, AddSelectClause<T>(sql), args))
				{
					IDataReader r;
					try
					{
						r = cmd.ExecuteReader();
					}
					catch (Exception x)
					{
						OnException(x);
						throw;
					}
					while (r.Read())
					{
						yield return CreatePoco<T>(r);
					}
				}
			}
		}
  
		public T Single<T>(string sql, params object[] args) where T : new()
		{
			return Query<T>(sql, args).Single();
		}
		public T SingleOrDefault<T>(string sql, params object[] args) where T : new()
		{
			return Fetch<T>(sql, args).SingleOrDefault();
		}
		public T First<T>(string sql, params object[] args) where T : new()
		{
			return Query<T>(sql, args).First();
		}
		public T FirstOrDefault<T>(string sql, params object[] args) where T : new()
		{
			return Query<T>(sql, args).First();
		}


		public List<T> Fetch<T>(Sql sql) where T : new()
		{
			return Fetch<T>(sql.SQL, sql.Arguments);
		}

		public IEnumerable<T> Query<T>(Sql sql) where T : new()
		{
			return Query<T>(sql.SQL, sql.Arguments);
		}

		public T Single<T>(Sql sql) where T : new()
		{
			return Query<T>(sql).Single();
		}
		public T SingleOrDefault<T>(Sql sql) where T : new()
		{
			return Query<T>(sql).SingleOrDefault();
		}
		public T First<T>(Sql sql) where T : new()
		{
			return Query<T>(sql).First();
		}
		public T FirstOrDefault<T>(Sql sql) where T : new()
		{
			return Query<T>(sql).First();
		}

	
		// Insert a poco into a table.  If the poco has a property with the same name 
		// as the primary key the id of the new record is assigned to it.  Either way,
 		// the new id is returned.
		public object Insert(string tableName, string primaryKey, object poco)
		{
			try
			{
				using (var conn = OpenSharedConnection())
				{
					using (var cmd = CreateCommand(conn, ""))
					{
						var pd = PocoData.ForType(poco.GetType());
						var names = new List<string>();
						var values = new List<string>();
						var index = 0;
						foreach (var i in pd.Columns)
						{
							// Don't insert the primary key
							if (primaryKey != null && i.Key == primaryKey)
								continue;

							names.Add(i.Key);
							values.Add(string.Format("{0}{1}", _paramPrefix, index++));
							cmd.AddParam(i.Value.GetValue(poco, null), _paramPrefix);
						}

						cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT @@IDENTITY AS NewID;",
								tableName,
								string.Join(",", names.ToArray()),
								string.Join(",", values.ToArray())
								);

						_lastSql = cmd.CommandText;
						_lastArgs = values.ToArray();

						// Insert the record, should get back it's ID
						var id = cmd.ExecuteScalar();

						// Assign the ID back to the primary key property
						if (primaryKey != null)
						{
							PropertyInfo piKey;
							if (pd.Columns.TryGetValue(primaryKey, out piKey))
							{
								piKey.SetValue(poco, id, null);
							}
						}

						return id;
					}
				}
			}
			catch (Exception x)
			{
				OnException(x);
				throw;
			}
		}

		// Insert an annotated poco object
		public object Insert(object poco)
		{
			var pd=new PocoData(poco.GetType());
			return Insert(pd.TableName, pd.PrimaryKey, poco);
		}
		
		public void Update(string tableName, string primaryKeyName, object poco)
		{
			Update(tableName, primaryKeyName, poco, null);
		}


		// Update a record with values from a poco.  primary key value can be either supplied or read from the poco
		public void Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
		{
			try
			{
				using (var conn = OpenSharedConnection())
				{
					using (var cmd = CreateCommand(conn, ""))
					{
						var sb = new StringBuilder();
						var index = 0;
						var pd = PocoData.ForType(poco.GetType());
						foreach (var i in pd.Columns)
						{
							// Don't update the primary key, but grab the value if we don't have it
							if (i.Key == primaryKeyName)
							{
								if (primaryKeyValue==null)
									primaryKeyValue=i.Value.GetValue(poco, null);
								continue;
							}

							// Build the sql
							if (index > 0)
								sb.Append(", ");
							sb.AppendFormat("{0} = {1}{2}", i.Key, _paramPrefix, index++);

							// Store the parameter in the command
							cmd.AddParam(i.Value.GetValue(poco, null), _paramPrefix);
						}

						cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
								tableName,
								sb.ToString(),
								primaryKeyName,
								_paramPrefix,
								index++
								);
						cmd.AddParam(primaryKeyValue, _paramPrefix);

						_lastSql = cmd.CommandText;
						_lastArgs = new object[] { primaryKeyValue };

						// Do it
						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception x)
			{
				OnException(x);
				throw;
			}
		}

		public void Update(object poco)
		{
			Update(poco, null);
		}

		// Update an annotated poco object
		public void Update(object poco, object primaryKeyValue)
		{
			var pd = new PocoData(poco.GetType());
			Update(pd.TableName, pd.PrimaryKey, poco, primaryKeyValue);
		}

		public void Delete(string tableName, string primaryKeyName, object poco)
		{
			Delete(tableName, primaryKeyName, poco, null);
		}

		// Delete a record, using the primary key value from a poco, or supplied
		public void Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
		{
			// If primary key value not specified, pick it up from the object
			if (primaryKeyValue == null)
			{
				var pi = poco.GetType().GetProperty(primaryKeyName);
				primaryKeyValue = pi.GetValue(poco, null);
			}

			// Do it
			var sql=string.Format("DELETE FROM {0} WHERE {1}=@0", tableName, primaryKeyName);
			Execute(sql, primaryKeyValue);
		}

		// Delete an annotated poco
		public void Delete(object poco)
		{
			var pd = new PocoData(poco.GetType());
			Delete(pd.TableName, pd.PrimaryKey, poco);
		}

		// Delete given a where clause
		public void Delete<T>(string Where, params object[] args)
		{
			var pd = new PocoData(typeof(T));
			Execute(string.Format("DELETE FROM {0} {1}", pd.TableName, Where), args);
		}

		// Check if a poco represents a new record
		public bool IsNew(string primaryKeyName, object poco)
		{
			// Get the property info for the primary key column
			var pi = poco.GetType().GetProperty(primaryKeyName);
			if (pi == null)
				throw new ArgumentException("The object doesn't have a property matching the primary key column name '{0}'", primaryKeyName);

			// Get it's value
			var pk = pi.GetValue(poco, null);
			if (pk==null)
				return true;

			var type=pk.GetType();

			if (type.IsValueType)
			{
				// Common primary key types
				if (type == typeof(long))
					return (long)pk == 0;
				else if (type == typeof(ulong))
					return (ulong)pk == 0;
				else if (type == typeof(int))
					return (int)pk == 0;
				else if (type == typeof(uint))
					return (int)pk == 0;

				// Create a default instance and compare
				return pk == Activator.CreateInstance(pk.GetType());
			}
			else
			{
				return pk==null;
			}
		}

		// Same as above but for decorated pocos
		public bool IsNew(object poco)
		{
			var pd = new PocoData(poco.GetType());
			return IsNew(pd.PrimaryKey, poco);
		}

		// Insert new record or Update existing record
		public void Save(string tableName, string primaryKeyName, object poco)
		{
			if (IsNew(primaryKeyName, poco))
			{
				Insert(tableName, primaryKeyName, poco);
			}
			else
			{
				Update(tableName, primaryKeyName, poco);
			}
		}

		// Same as above for decorated pocos
		public void Save(object poco)
		{
			var pd = new PocoData(poco.GetType());
			Save(pd.TableName, pd.PrimaryKey, poco);
		}

		public string LastSQL {	get	{ return _lastSql; } }
		public object[] LastArgs { get { return _lastArgs; } }

		public string LastCommand
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append(_lastSql);
				sb.Append("\r\n\r\n");
				for (int i = 0; i < _lastArgs.Length; i++)
				{
					sb.AppendFormat("{0} - {1}\r\n", i, _lastArgs[i].ToString());
				}
				return sb.ToString();
			}
		}



		// Member variables
		string _connectionString;
		DbProviderFactory _factory;
		DbConnection _sharedConnection;
		DbTransaction _transaction;
		int _transactionDepth;
		string _lastSql;
		object[] _lastArgs;
	}

	// Transaction object helps maintain transaction depth counts
	public class Transaction : IDisposable
	{
		public Transaction(Database db)
		{
			_db = db;
			_db.BeginTransaction();
		}

		public void Complete()
		{
			_db.CompleteTransaction();
			_db = null;
		}

		public void Dispose()
		{
			if (_db != null)
				_db.AbortTransaction();
		}

		Database _db;
	}

	// Simple helper class for building SQL statments
	// eg:
	//   new Sql()
	//		.Select("id", "title")
	//		.From("articles")
	//		.Where("date_created>@0", DateTime.UtcNow)
	//		.OrderBy("title")
	public class Sql
	{
		public Sql()
		{
		}

		public Sql(string sqlStart, string sql, params object[] args)
		{
			if (!String.IsNullOrEmpty(sqlStart) && !sql.StartsWith(sqlStart, StringComparison.InvariantCultureIgnoreCase))
			{
				_sql = sqlStart + " " + sql;
			}
			else
			{
				_sql = sql;
			}
			_args = args;
		}

		string _sql;
		object[] _args;
		Sql _rhs;
		string _sqlFinal;
		object[] _argsFinal;

		void Build()
		{
			// already built?
			if (_sqlFinal!=null)
				return;

			// Build it
			var sb = new StringBuilder();
			var args = new List<object>();
			Build(sb, args);
			_sqlFinal=sb.ToString();
			_argsFinal=args.ToArray();
		}

		public string SQL
		{
			get
			{
				Build();
				return _sqlFinal;
			}
		}

		public object[] Arguments
		{
			get
			{
				Build();
				return _argsFinal;
			}
		}

		public Sql Append(Sql sql)
		{
			if (_rhs != null)
				_rhs.Append(sql);
			else
				_rhs = sql;

			return this;
		}

		public Sql Append(string sql, params object[] args)
		{
			return Append(new Sql("", sql, args));
		}

		public Sql Where(string sql, params object[] args)
		{
			return Append(new Sql("WHERE", sql, args));
		}

		public Sql OrderBy(params object[] args)
		{
			return Append(new Sql("", "ORDER BY " + String.Join(", ", (from x in args select x.ToString()).ToArray())));
		}

		public Sql Select(params object[] args)
		{
			return Append(new Sql("", "SELECT " + String.Join(", ", (from x in args select x.ToString()).ToArray())));
		}

		public Sql From(params object[] args)
		{
			return Append(new Sql("", "FROM " + String.Join(", ", (from x in args select x.ToString()).ToArray())));
		}

		public void Build(StringBuilder sb, List<object> args)
		{
			if (!String.IsNullOrEmpty(_sql))
			{
				// Add SQL to the string
				if (sb.Length > 0)
				{
					sb.Append("\n");
				}

				var rxParams = new Regex(@"(?<!@)@\w+");
				var sql = rxParams.Replace(_sql, m =>
				{
					string param = m.Value.Substring(1);

					int paramIndex;
					if (int.TryParse(param, out paramIndex))
					{
						// Numbered parameter
						if (paramIndex < 0 || paramIndex >= _args.Length)
						{
							throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, _args.Length, _sql));
						}

						args.Add(_args[paramIndex]);

					}
					else
					{
						// Look for a property on one of the arguments with this name
						bool found = false;
						foreach (var o in _args)
						{
							var pi = o.GetType().GetProperty(param);
							if (pi != null)
							{
								args.Add(pi.GetValue(o, null));
								found = true;
								break;
							}
						}

						// Check found
						if (!found)
						{
							throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, _sql));
						}
					}
					return "@" + (args.Count-1).ToString();
				}
				);

				sb.Append(sql);
			}

			// Do rhs first
			if (_rhs != null)
				_rhs.Build(sb, args);
		}
	}
}
