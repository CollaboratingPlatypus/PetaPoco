using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PetaPoco;

namespace PetaPoco.Tests
{
	[TestFixture]
	public class SqlBuilder : AssertionHelper
	{
		public SqlBuilder()
		{
		}

		[Test]
		public void simple_append()
		{
			var sql = new Sql();
			sql.Append("LINE 1");
			sql.Append("LINE 2");
			sql.Append("LINE 3");

			Expect(sql.SQL, Is.EqualTo("LINE 1\nLINE 2\nLINE 3"));
			Expect(sql.Arguments.Length, Is.EqualTo(0));
		}

		[Test]
		public void single_arg()
		{
			var sql = new Sql();
			sql.Append("arg @0", "a1");

			Expect(sql.SQL, Is.EqualTo("arg @0"));
			Expect(sql.Arguments.Length, Is.EqualTo(1));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
		}

		[Test]
		public void multiple_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @1", "a1", "a2");

			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
			Expect(sql.Arguments.Length, Is.EqualTo(2));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
			Expect(sql.Arguments[1], Is.EqualTo("a2"));
		}

		[Test]
		public void unused_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @2", "a1", "a2", "a3");

			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
			Expect(sql.Arguments.Length, Is.EqualTo(2));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
			Expect(sql.Arguments[1], Is.EqualTo("a3"));
		}

		[Test]
		public void unordered_args()
		{
			var sql = new Sql();
			sql.Append("arg @2 @1", "a1", "a2", "a3");

			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
			Expect(sql.Arguments.Length, Is.EqualTo(2));
			Expect(sql.Arguments[0], Is.EqualTo("a3"));
			Expect(sql.Arguments[1], Is.EqualTo("a2"));
		}

		[Test]
		public void repeated_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @1 @0 @1", "a1", "a2");

			Expect(sql.SQL, Is.EqualTo("arg @0 @1 @2 @3"));
			Expect(sql.Arguments.Length, Is.EqualTo(4));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
			Expect(sql.Arguments[1], Is.EqualTo("a2"));
			Expect(sql.Arguments[2], Is.EqualTo("a1"));
			Expect(sql.Arguments[3], Is.EqualTo("a2"));
		}

		[Test]
		public void mysql_user_vars()
		{
			var sql = new Sql();
			sql.Append("arg @@user1 @2 @1 @@@system1", "a1", "a2", "a3");

			Expect(sql.SQL, Is.EqualTo("arg @@user1 @0 @1 @@@system1"));
			Expect(sql.Arguments.Length, Is.EqualTo(2));
			Expect(sql.Arguments[0], Is.EqualTo("a3"));
			Expect(sql.Arguments[1], Is.EqualTo("a2"));
		}

		[Test]
		public void named_args()
		{
			var sql = new Sql();
			sql.Append("arg @name @password", new { name = "n", password = "p" });

			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
			Expect(sql.Arguments.Length, Is.EqualTo(2));
			Expect(sql.Arguments[0], Is.EqualTo("n"));
			Expect(sql.Arguments[1], Is.EqualTo("p"));
		}



		[Test]
		public void mixed_named_and_numbered_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @name @1 @password @2", "a1", "a2", "a3", new { name = "n", password = "p" });

			Expect(sql.SQL, Is.EqualTo("arg @0 @1 @2 @3 @4"));
			Expect(sql.Arguments.Length, Is.EqualTo(5));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
			Expect(sql.Arguments[1], Is.EqualTo("n"));
			Expect(sql.Arguments[2], Is.EqualTo("a2"));
			Expect(sql.Arguments[3], Is.EqualTo("p"));
			Expect(sql.Arguments[4], Is.EqualTo("a3"));
		}

		[Test]
		public void append_with_args()
		{
			var sql = new Sql();
			sql.Append("l1 @0", "a0");
			sql.Append("l2 @0", "a1");
			sql.Append("l3 @0", "a2");

			Expect(sql.SQL, Is.EqualTo("l1 @0\nl2 @1\nl3 @2"));
			Expect(sql.Arguments.Length, Is.EqualTo(3));
			Expect(sql.Arguments[0], Is.EqualTo("a0"));
			Expect(sql.Arguments[1], Is.EqualTo("a1"));
			Expect(sql.Arguments[2], Is.EqualTo("a2"));
		}

		[Test]
		public void append_with_args2()
		{
			var sql = new Sql();
			sql.Append("l1");
			sql.Append("l2 @0 @1", "a1", "a2");
			sql.Append("l3 @0", "a3");

			Expect(sql.SQL, Is.EqualTo("l1\nl2 @0 @1\nl3 @2"));
			Expect(sql.Arguments.Length, Is.EqualTo(3));
			Expect(sql.Arguments[0], Is.EqualTo("a1"));
			Expect(sql.Arguments[1], Is.EqualTo("a2"));
			Expect(sql.Arguments[2], Is.EqualTo("a3"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void invalid_arg_index()
		{
			var sql = new Sql();
			sql.Append("arg @0 @1", "a0");
			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void invalid_arg_name()
		{
			var sql = new Sql();
			sql.Append("arg @name1 @name2", new { x = 1, y = 2 });
			Expect(sql.SQL, Is.EqualTo("arg @0 @1"));
		}

		[Test]
		public void append_instances()
		{
			var sql = new Sql("l0 @0", "a0");
			var sql1 = new Sql("l1 @0", "a1");
			var sql2 = new Sql("l2 @0", "a2");

			Expect(sql.Append(sql1), Is.SameAs(sql));
			Expect(sql.Append(sql2), Is.SameAs(sql));

			Expect(sql.SQL, Is.EqualTo("l0 @0\nl1 @1\nl2 @2"));
			Expect(sql.Arguments.Length, Is.EqualTo(3));
			Expect(sql.Arguments[0], Is.EqualTo("a0"));
			Expect(sql.Arguments[1], Is.EqualTo("a1"));
			Expect(sql.Arguments[2], Is.EqualTo("a2"));
		}

		[Test]
		public void ConsecutiveWhere()
		{
			var sql = new Sql()
						.Append("SELECT * FROM blah");

			sql.Append("WHERE x");
			sql.Append("WHERE y");

			Expect(sql.SQL, Is.EqualTo("SELECT * FROM blah\nWHERE x\nAND y"));
		}

		[Test]
		public void ConsecutiveOrderBy()
		{
			var sql = new Sql()
						.Append("SELECT * FROM blah");

			sql.Append("ORDER BY x");
			sql.Append("ORDER BY y");

			Expect(sql.SQL, Is.EqualTo("SELECT * FROM blah\nORDER BY x\n, y"));
		}

		[Test]
		public void param_expansion_1()
		{
			// Simple collection parameter expansion
			var sql = Sql.Builder.Append("@0 IN (@1) @2", 20, new int[] { 1, 2, 3 }, 30);
			Expect(sql.SQL, Is.EqualTo("@0 IN (@1,@2,@3) @4"));
			Expect(sql.Arguments.Length, Is.EqualTo(5));
			Expect(sql.Arguments[0], Is.EqualTo(20));
			Expect(sql.Arguments[1], Is.EqualTo(1));
			Expect(sql.Arguments[2], Is.EqualTo(2));
			Expect(sql.Arguments[3], Is.EqualTo(3));
			Expect(sql.Arguments[4], Is.EqualTo(30));
		}

		[Test]
		public void param_expansion_2()
		{
			// Out of order expansion
			var sql = Sql.Builder.Append("IN (@3) (@1)", null, new int[] { 1, 2, 3 }, null, new int[] { 4, 5, 6 });
			Expect(sql.SQL, Is.EqualTo("IN (@0,@1,@2) (@3,@4,@5)"));
			Expect(sql.Arguments.Length, Is.EqualTo(6));
			Expect(sql.Arguments[0], Is.EqualTo(4));
			Expect(sql.Arguments[1], Is.EqualTo(5));
			Expect(sql.Arguments[2], Is.EqualTo(6));
			Expect(sql.Arguments[3], Is.EqualTo(1));
			Expect(sql.Arguments[4], Is.EqualTo(2));
			Expect(sql.Arguments[5], Is.EqualTo(3));
		}

		[Test]
		public void param_expansion_named()
		{
			// Expand a named parameter
			var sql = Sql.Builder.Append("IN (@numbers)", new { numbers = (new int[] { 1, 2, 3 }) } );
			Expect(sql.SQL, Is.EqualTo("IN (@0,@1,@2)"));
			Expect(sql.Arguments.Length, Is.EqualTo(3));
			Expect(sql.Arguments[0], Is.EqualTo(1));
			Expect(sql.Arguments[1], Is.EqualTo(2));
			Expect(sql.Arguments[2], Is.EqualTo(3));
		}

		[Test]
		public void join()
		{
			var sql = Sql.Builder
				.Select("*")
				.From("articles")
				.LeftJoin("comments").On("articles.article_id=comments.article_id");
			Expect(sql.SQL, Is.EqualTo("SELECT *\nFROM articles\nLEFT JOIN comments\nON articles.article_id=comments.article_id"));
		}

	}

}

