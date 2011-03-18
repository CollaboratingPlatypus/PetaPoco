using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace PetaPoco
{
	// Attach [Peta.Ignore] to poco class properties to be ignored
	[AttributeUsage(AttributeTargets.Property)]
	public class Ignore : Attribute
	{
		public Ignore()
		{

		}
	}

	// Attach [Peta.TableName("tableName")] to poco classes
	[AttributeUsage(AttributeTargets.Class)]
	public class TableName : Attribute
	{
		public TableName(string tableName)
		{
			Value = tableName;
		}

		public string Value { get; private set; }
	}

	// Attach [Peta.PrimaryKey("primaryKey")] to poco classes
	[AttributeUsage(AttributeTargets.Class)]
	public class PrimaryKey : Attribute
	{
		public PrimaryKey(string primaryKey)
		{
			Value = primaryKey;
		}

		public string Value { get; private set; }
	}

	// Helper to grab the poco attribute settings from a poco
	public class PocoData
	{
		public PocoData(Type t)
		{
			// Get the table name
			var a = t.GetCustomAttributes(typeof(TableName), true);
			if (a.Length == 0)
			{
				TableName = t.Name;
			}
			else
			{
				TableName = (a[0] as TableName).Value;
			}

			// Get the primary key
			a = t.GetCustomAttributes(typeof(PrimaryKey), true);
			if (a.Length == 0)
			{
				PrimaryKey = "ID";
			}
			else
			{
				PrimaryKey = (a[0] as PrimaryKey).Value;
			}
		}

		public string TableName { get; private set; }
		public string PrimaryKey { get; private set; }
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

		// Get the PropertyInfo for a property with a specified name, checking for [Ignore]
		public static System.Reflection.PropertyInfo GetPocoProperty(this Type type, string name)
		{
			var pi=type.GetProperty(name);

			// Check it doesn't have a Peta.Ignore attribute
			if (pi!=null && pi.PocoIgnore())
				return null;

			return pi;
		}

		// Check if property should be ignored
		public static bool PocoIgnore(this System.Reflection.PropertyInfo pi)
		{
			var ignore = pi.GetCustomAttributes(typeof(Ignore), true);
			return ignore.Length > 0;
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
			}

		}

		// Internal helper to cleanup transaction stuff
		void CleanupTransaction()
		{
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
			var type = typeof(T);

			for (var i = 0; i < r.FieldCount; i++)
			{
				// Get property info
				var name=r.GetName(i);
				var pi=type.GetPocoProperty(name);
				object val=r[i];

				if (pi != null)
				{
					if (Nullable.GetUnderlyingType(pi.PropertyType) != null && val!=null && val.GetType() == typeof(DBNull))
					{
						val=null;
					}

					pi.SetValue(record, val, null);
				}
			}

			return record;			
		}

		// Execute a non-query command
		public void Execute(string sql, params object[] args)
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, sql, args))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		string _paramPrefix = "@";

		public void Execute(Sql sql)
		{
			sql.ParameterPrefix = _paramPrefix;
			Execute(sql.SQL, sql.Arguments);
		}

		// Execute and cast a scalar property
		public T ExecuteScalar<T>(string sql, params object[] args)
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, sql, args))
				{
					object val=cmd.ExecuteScalar();
					return (T)val;
				}
			}
		}

		public T ExecuteScalar<T>(Sql sql)
		{
			sql.ParameterPrefix = _paramPrefix;
			return ExecuteScalar<T>(sql.SQL, sql.Arguments);
		}

		// Return a typed list of pocos
        public List<T> Fetch<T>(string sql, params object[] args) where T:new()
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, sql, args))
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

		// Return an enumerable collection of pocos
		public IEnumerable<T> Query<T>(string sql, params object[] args) where T:new()
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, sql, args))
				{
					var r = cmd.ExecuteReader();
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
			return Query<T>(sql, args).SingleOrDefault();
		}
		public T First<T>(string sql, params object[] args) where T : new()
		{
			return Query<T>(sql, args).First();
		}
		public T FirstOrDefault<T>(string sql, params object[] args) where T : new()
		{
			return Query<T>(sql, args).First();
		}


		public IEnumerable<T> Fetch<T>(Sql sql) where T : new()
		{
			sql.ParameterPrefix = _paramPrefix;
			return Fetch<T>(sql.SQL, sql.Arguments);
		}

		public IEnumerable<T> Query<T>(Sql sql) where T : new()
		{
			sql.ParameterPrefix = _paramPrefix;
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

	
		// Helper methods to pull all the public properties names and values from a poco
		class NameValue
		{
			public string name;
			public object value;
		}
		IEnumerable<NameValue> GetProperties(object o)
		{
			if (o == null)
				yield break;

			foreach (var pi in o.GetType().GetProperties())
			{
				if (pi.PocoIgnore())
					continue;

				yield return new NameValue { name=pi.Name, value = pi.GetValue(o, null)};
			}
		}

		// Insert a poco into a table.  If the poco has a property with the same name 
		// as the primary key the id of the new record is assigned to it.  Either way,
 		// the new id is returned.
		public object Insert(string tableName, string primaryKey, object poco)
		{
			using (var conn=OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, ""))
				{
					var names = new List<string>();
					var values = new List<string>();
					var index = 0;
					foreach (var i in GetProperties(poco))
					{
						// Don't insert the primary key
						if (primaryKey!=null && i.name == primaryKey)
							continue;

						names.Add(i.name);
						values.Add(string.Format("{0}{1}", _paramPrefix, index++));
						cmd.AddParam(i.value, _paramPrefix);
					}

					cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT @@IDENTITY AS NewID;", 
							tableName, 
							string.Join(",", names.ToArray()),
							string.Join(",", values.ToArray())
							);

					// Insert the record, should get back it's ID
					var id = cmd.ExecuteScalar();

					// Assign the ID back to the primary key property
					if (primaryKey!=null)
					{
						var piKey=poco.GetType().GetPocoProperty(primaryKey);
						if (piKey != null)
						{
							piKey.SetValue(poco, id, null);
						}
					}

					return id;
				}
			}
		}

		// Insert an annotated poco object
		public object Insert(object poco)
		{
			var pd=new PocoData(poco.GetType());
			return Insert(pd.TableName, pd.PrimaryKey, poco);
		}

		// Update a record with values from a poco.  primary key value can be either supplied or read from the poco
		public void Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue=null)
		{
			using (var conn = OpenSharedConnection())
			{
				using (var cmd = CreateCommand(conn, ""))
				{
					var sb = new StringBuilder();
					var index = 0;
					foreach (var i in GetProperties(poco))
					{
						// Don't update the primary key, but grab the value if we don't have it
						if (i.name == primaryKeyName)
						{
							if (primaryKeyValue==null)
								primaryKeyValue=i.value;
							continue;
						}

						// Build the sql
						if (index > 0)
							sb.Append(", ");
						sb.AppendFormat("{0} = {1}{2}", i.name, _paramPrefix, index++);

						// Store the parameter in the command
						cmd.AddParam(i.value, _paramPrefix);
					}

					cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
							tableName,
							sb.ToString(),
							primaryKeyName,
							_paramPrefix,
							index++
							);
					cmd.AddParam(primaryKeyValue, _paramPrefix);

					// Do it
					cmd.ExecuteNonQuery();
				}
			}
		}

		// Update an annotated poco object
		public void Update(object poco, object primaryKeyValue=null)
		{
			var pd = new PocoData(poco.GetType());
			Update(pd.TableName, pd.PrimaryKey, poco, primaryKeyValue);
		}

		// Delete a record, using the primary key value from a poco, or supplied
		public void Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue = null)
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

		// Member variables
		string _connectionString;
		DbProviderFactory _factory;
		DbConnection _sharedConnection;
		DbTransaction _transaction;
		int _transactionDepth;
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
			Build(sb, args, ParameterPrefix==null ? "@" : ParameterPrefix);
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

		public string ParameterPrefix
		{
			get;
			set;
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

		public void Build(StringBuilder sb, List<object> args, string ParamPrefix)
		{
			if (!String.IsNullOrEmpty(_sql))
			{
				// Add SQL to the string
				if (sb.Length > 0)
				{
					sb.Append("\n");
				}

				System.Text.RegularExpressions.Regex rxParams = new System.Text.RegularExpressions.Regex(@"(?<!@)@\w+");
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
					return ParamPrefix + (args.Count-1).ToString();
				}
				);

				// If using MySql parameter format because user-variables are supported, fixup @@=>@ and @@@=>@@
				if (ParamPrefix=="?")
					sql = sql.Replace("@@", "@");

				sb.Append(sql);
			}

			// Do rhs first
			if (_rhs != null)
				_rhs.Build(sb, args, ParamPrefix);
		}
	}
}
