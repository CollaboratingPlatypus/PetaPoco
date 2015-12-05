// <copyright file="Sql.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco.Internal;

namespace PetaPoco
{
    /// <summary>
    ///     A simple helper class for build SQL statements
    /// </summary>
    public class Sql
    {
        private object[] _args;
        private object[] _argsFinal;
        private Sql _rhs;

        private string _sql;
        private string _sqlFinal;

        /// <summary>
        ///     Instantiate a new SQL Builder object.  Weirdly implemented as a property but makes
        ///     for more elegantly readble fluent style construction of SQL Statements
        ///     eg: db.Query(Sql.Builder.Append(....))
        /// </summary>
        public static Sql Builder
        {
            get { return new Sql(); }
        }

        /// <summary>
        ///     Returns the final SQL statement represented by this builder
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
        ///     Gets the complete, final set of arguments collected by this builder.
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
        ///     Default, empty constructor
        /// </summary>
        public Sql()
        {
        }

        /// <summary>
        ///     Construct an SQL statement with the supplied SQL and arguments
        /// </summary>
        /// <param name="sql">The SQL statement or fragment</param>
        /// <param name="args">Arguments to any parameters embedded in the SQL</param>
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
        ///     Append another SQL builder instance to the right-hand-side of this SQL builder
        /// </summary>
        /// <param name="sql">A reference to another SQL builder instance</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql Append(Sql sql)
        {
            if (_rhs != null)
                _rhs.Append(sql);
            else
                _rhs = sql;

            return this;
        }

        /// <summary>
        ///     Append an SQL fragement to the right-hand-side of this SQL builder
        /// </summary>
        /// <param name="sql">The SQL statement or fragment</param>
        /// <param name="args">Arguments to any parameters embedded in the SQL</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
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
            if (!String.IsNullOrEmpty(_sql))
            {
                // Add SQL to the string
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                var sql = ParametersHelper.ProcessParams(_sql, _args, args);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                    sql = "AND " + sql.Substring(6);
                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                    sql = ", " + sql.Substring(9);

                sb.Append(sql);
            }

            // Now do rhs
            if (_rhs != null)
                _rhs.Build(sb, args, this);
        }

        /// <summary>
        ///     Appends an SQL WHERE clause to this SQL builder
        /// </summary>
        /// <param name="sql">The condition of the WHERE clause</param>
        /// <param name="args">Arguments to any parameters embedded in the supplied SQL</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql Where(string sql, params object[] args)
        {
            return Append(new Sql("WHERE (" + sql + ")", args));
        }

        /// <summary>
        ///     Appends an SQL ORDER BY clause to this SQL builder
        /// </summary>
        /// <param name="columns">A collection of SQL column names to order by</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql OrderBy(params object[] columns)
        {
            return Append(new Sql("ORDER BY " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        /// <summary>
        ///     Appends an SQL SELECT clause to this SQL builder
        /// </summary>
        /// <param name="columns">
        ///     A collection of SQL column names to select
        ///     <param>
        ///         <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql Select(params object[] columns)
        {
            return Append(new Sql("SELECT " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        /// <summary>
        ///     Appends an SQL FROM clause to this SQL builder
        /// </summary>
        /// <param name="tables">A collection of table names to be used in the FROM clause</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql From(params object[] tables)
        {
            return Append(new Sql("FROM " + String.Join(", ", (from x in tables select x.ToString()).ToArray())));
        }

        /// <summary>
        ///     Appends an SQL GROUP BY clause to this SQL builder
        /// </summary>
        /// <param name="columns">A collection of column names to be grouped by</param>
        /// <returns>A reference to this builder, allowing for fluent style concatenation</returns>
        public Sql GroupBy(params object[] columns)
        {
            return Append(new Sql("GROUP BY " + String.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        private SqlJoinClause Join(string JoinType, string table)
        {
            return new SqlJoinClause(Append(new Sql(JoinType + table)));
        }

        /// <summary>
        ///     Appends an SQL INNER JOIN clause to this SQL builder
        /// </summary>
        /// <param name="table">The name of the table to join</param>
        /// <returns>A reference an SqlJoinClause through which the join condition can be specified</returns>
        public SqlJoinClause InnerJoin(string table)
        {
            return Join("INNER JOIN ", table);
        }

        /// <summary>
        ///     Appends an SQL LEFT JOIN clause to this SQL builder
        /// </summary>
        /// <param name="table">The name of the table to join</param>
        /// <returns>A reference an SqlJoinClause through which the join condition can be specified</returns>
        public SqlJoinClause LeftJoin(string table)
        {
            return Join("LEFT JOIN ", table);
        }

        /// <summary>
        ///     The SqlJoinClause is a simple helper class used in the construction of SQL JOIN statements with the SQL builder
        /// </summary>
        public class SqlJoinClause
        {
            private readonly Sql _sql;

            public SqlJoinClause(Sql sql)
            {
                _sql = sql;
            }

            /// <summary>
            ///     Appends a SQL ON clause after a JOIN statement
            /// </summary>
            /// <param name="onClause">The ON clause to be appended</param>
            /// <param name="args">Arguments to any parameters embedded in the supplied SQL</param>
            /// <returns>A reference to the parent SQL builder, allowing for fluent style concatenation</returns>
            public Sql On(string onClause, params object[] args)
            {
                return _sql.Append("ON " + onClause, args);
            }
        }
    }
}