using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco.Internal;

namespace PetaPoco
{
    /// <summary>
    /// A simple helper class for building mutable SQL statements.
    /// </summary>
    public class Sql
    {
        private Sql _rhs;
        private string _sql;
        private string _sqlFinal;
        private object[] _args;
        private object[] _argsFinal;

        /// <summary>
        /// Gets a new initialized instance of the <see cref="Sql"/> builder class.
        /// </summary>
        /// <remarks>
        /// Weirdly implemented as a property, but makes for more elegant and readable fluent-style construction of SQL Statements:
        /// <br/><c>db.Query(Sql.Builder.Append(/*...*/));</c>.
        /// </remarks>
        public static Sql Builder
        {
            get { return new Sql(); }
        }

        /// <summary>
        /// Gets the final SQL statement stored in this builder.
        /// </summary>
        public string SQL
        {
            get
            {
                Build();
                return _sqlFinal;
            }
        }

        /// <summary>
        /// Gets the complete, final array of arguments collected by this builder.
        /// </summary>
        public object[] Arguments
        {
            get
            {
                Build();
                return _argsFinal;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Sql"/> builder class with default values.
        /// </summary>
        public Sql()
        {
        }

        /// <summary>
        /// Construct an SQL statement from the given SQL string and arguments.
        /// </summary>
        /// <param name="sql">The SQL clause or statement.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        public Sql(string sql, params object[] args)
        {
            _sql = sql;
            _args = args;
        }

        private void Build()
        {
            // already built?
            if (_sqlFinal != null)
                return;

            // Build it
            var sb = new StringBuilder();
            var args = new List<object>();
            Build(sb, args, null);
            _sqlFinal = sb.ToString();
            _argsFinal = args.ToArray();
        }

        /// <summary>
        /// Appends another Sql builder instance to the right-hand-side of this Sql builder.
        /// </summary>
        /// <param name="sql">An SQL builder instance.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql Append(Sql sql)
        {
            if (_rhs != null)
                _rhs.Append(sql);
            else
                _rhs = sql;

            _sqlFinal = null;
            return this;
        }

        /// <summary>
        /// Appends an SQL fragment to the right-hand-side of this Sql builder instance.
        /// </summary>
        /// <param name="sql">The SQL clause or statement.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql Append(string sql, params object[] args)
        {
            return Append(new Sql(sql, args));
        }

        private static bool Is(Sql sql, string sqltype)
        {
            return sql != null && sql._sql != null && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
        }

        private void Build(StringBuilder sb, List<object> args, Sql lhs)
        {
            if (!string.IsNullOrEmpty(_sql))
            {
                // Add SQL to the string
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                var sql = ParametersHelper.ProcessQueryParams(_sql, _args, args);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                    sql = "AND " + sql.Substring(6);
                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                    sql = ", " + sql.Substring(9);
                // add set clause
                if (Is(lhs, "SET ") && Is(this, "SET "))
                    sql = ", " + sql.Substring(4);

                sb.Append(sql);
            }

            // Now do rhs
            if (_rhs != null)
                _rhs.Build(sb, args, this);
        }

        /// <summary>
        /// Appends a <c>SET</c> clause to this Sql builder.
        /// </summary>
        /// <param name="sql">The SQL string representing the assignment portion of the SET clause: <c>{field} = {value}</c>.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql Set(string sql, params object[] args)
        {
            return Append(new Sql("SET " + sql, args));
        }

        /// <summary>
        /// Appends a <c>WHERE</c> clause to this Sql builder.
        /// </summary>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause: <c>{field} = {value}</c>.</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql Where(string sql, params object[] args)
        {
            return Append(new Sql("WHERE (" + sql + ")", args));
        }

        /// <summary>
        /// Appends an <c>ORDER BY</c> clause to this Sql builder.
        /// </summary>
        /// <param name="columns">The column names to order by.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql OrderBy(params object[] columns)
        {
            return Append(new Sql("ORDER BY " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        /// <summary>
        /// Appends a <c>SELECT</c> clause to this Sql builder.
        /// </summary>
        /// <param name="columns">The column names to include in the SELECT clause.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql Select(params object[] columns)
        {
            return Append(new Sql("SELECT " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        /// <summary>
        /// Appends a <c>FROM</c> clause to this Sql builder.
        /// </summary>
        /// <param name="tables">The table names to include in the FROM clause.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql From(params object[] tables)
        {
            return Append(new Sql("FROM " + string.Join(", ", (from x in tables select x.ToString()).ToArray())));
        }

        /// <summary>
        /// Appends a <c>GROUP BY</c> clause to this Sql builder.
        /// </summary>
        /// <param name="columns">The column names to group by.</param>
        /// <returns>This Sql builder instance, allowing for fluent style concatenation.</returns>
        public Sql GroupBy(params object[] columns)
        {
            return Append(new Sql("GROUP BY " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        private SqlJoinClause Join(string joinType, string table)
        {
            return new SqlJoinClause(Append(new Sql(joinType + table)));
        }

        /// <summary>
        /// Appends an <c>INNER JOIN</c> clause to this Sql builder.
        /// </summary>
        /// <param name="tableName">The name of the table to join.</param>
        /// <returns>An SqlJoinClause instance, to be used to append the JOIN conditions.</returns>
        public SqlJoinClause InnerJoin(string tableName)
        {
            return Join("INNER JOIN ", tableName);
        }

        /// <summary>
        /// Appends a <c>LEFT JOIN</c> clause to this Sql builder.
        /// </summary>
        /// <param name="tableName">The name of the table to join.</param>
        /// <returns>An SqlJoinClause instance, to be used to append the JOIN conditions.</returns>
        public SqlJoinClause LeftJoin(string tableName)
        {
            return Join("LEFT JOIN ", tableName);
        }

        /// <summary>
        /// Returns the complete SQL statement represented by this builder.
        /// </summary>
        /// <returns>The complete SQL statement.</returns>
        public override string ToString()
        {
            return SQL;
        }

        /// <summary>
        /// The SqlJoinClause is a simple <see cref="Sql"/> builder helper class used to build <c>JOIN</c> clauses.
        /// </summary>
        public class SqlJoinClause
        {
            private readonly Sql _sql;

            /// <summary>
            /// Creates a new <see cref="SqlJoinClause"/> instance from the specified <see cref="Sql"/> builder.
            /// </summary>
            /// <param name="sql">An SQL builder instance.</param>
            public SqlJoinClause(Sql sql)
            {
                _sql = sql;
            }

            /// <summary>
            /// Appends an <c>ON</c> expression to the <c>JOIN</c> clause.
            /// </summary>
            /// <param name="onClause">The SQL expression defining the ON condition for the JOIN clause: <c>{table1}.{column1} = {table2}.{column2}</c>.</param>
            /// <param name="args">The parameters to embed in the SQL string.</param>
            /// <returns>The parent Sql builder, allowing for fluent style concatenation.</returns>
            public Sql On(string onClause, params object[] args)
            {
                return _sql.Append("ON " + onClause, args);
            }
        }
    }
}
