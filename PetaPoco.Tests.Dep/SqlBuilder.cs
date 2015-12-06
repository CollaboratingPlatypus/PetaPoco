using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using PetaPoco;

namespace PetaPoco.Tests
{
	[TestFixture]
	public class SqlBuilder
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

			Assert.AreEqual(sql.SQL, "LINE 1\nLINE 2\nLINE 3");
			Assert.AreEqual(sql.Arguments.Length, 0);
		}

		[Test]
		public void single_arg()
		{
			var sql = new Sql();
			sql.Append("arg @0", "a1");

			Assert.AreEqual(sql.SQL, "arg @0");
			Assert.AreEqual(sql.Arguments.Length, 1);
			Assert.AreEqual(sql.Arguments[0], "a1");
		}

		[Test]
		public void multiple_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @1", "a1", "a2");

			Assert.AreEqual(sql.SQL, "arg @0 @1");
			Assert.AreEqual(sql.Arguments.Length, 2);
			Assert.AreEqual(sql.Arguments[0], "a1");
			Assert.AreEqual(sql.Arguments[1], "a2");
		}

		[Test]
		public void unused_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @2", "a1", "a2", "a3");

			Assert.AreEqual(sql.SQL, "arg @0 @1");
			Assert.AreEqual(sql.Arguments.Length, 2);
			Assert.AreEqual(sql.Arguments[0], "a1");
			Assert.AreEqual(sql.Arguments[1], "a3");
		}

		[Test]
		public void unordered_args()
		{
			var sql = new Sql();
			sql.Append("arg @2 @1", "a1", "a2", "a3");

			Assert.AreEqual(sql.SQL, "arg @0 @1");
			Assert.AreEqual(sql.Arguments.Length, 2);
			Assert.AreEqual(sql.Arguments[0], "a3");
			Assert.AreEqual(sql.Arguments[1], "a2");
		}

		[Test]
		public void repeated_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @1 @0 @1", "a1", "a2");

			Assert.AreEqual(sql.SQL, "arg @0 @1 @2 @3");
			Assert.AreEqual(sql.Arguments.Length, 4);
			Assert.AreEqual(sql.Arguments[0], "a1");
			Assert.AreEqual(sql.Arguments[1], "a2");
			Assert.AreEqual(sql.Arguments[2], "a1");
			Assert.AreEqual(sql.Arguments[3], "a2");
		}

		[Test]
		public void mysql_user_vars()
		{
			var sql = new Sql();
			sql.Append("arg @@user1 @2 @1 @@@system1", "a1", "a2", "a3");

			Assert.AreEqual(sql.SQL, "arg @@user1 @0 @1 @@@system1");
			Assert.AreEqual(sql.Arguments.Length, 2);
			Assert.AreEqual(sql.Arguments[0], "a3");
			Assert.AreEqual(sql.Arguments[1], "a2");
		}

		[Test]
		public void named_args()
		{
			var sql = new Sql();
			sql.Append("arg @name @password", new { name = "n", password = "p" });

			Assert.AreEqual(sql.SQL, "arg @0 @1");
			Assert.AreEqual(sql.Arguments.Length, 2);
			Assert.AreEqual(sql.Arguments[0], "n");
			Assert.AreEqual(sql.Arguments[1], "p");
		}



		[Test]
		public void mixed_named_and_numbered_args()
		{
			var sql = new Sql();
			sql.Append("arg @0 @name @1 @password @2", "a1", "a2", "a3", new { name = "n", password = "p" });

			Assert.AreEqual(sql.SQL, "arg @0 @1 @2 @3 @4");
			Assert.AreEqual(sql.Arguments.Length, 5);
			Assert.AreEqual(sql.Arguments[0], "a1");
			Assert.AreEqual(sql.Arguments[1], "n");
			Assert.AreEqual(sql.Arguments[2], "a2");
			Assert.AreEqual(sql.Arguments[3], "p");
			Assert.AreEqual(sql.Arguments[4], "a3");
		}

		[Test]
		public void append_with_args()
		{
			var sql = new Sql();
			sql.Append("l1 @0", "a0");
			sql.Append("l2 @0", "a1");
			sql.Append("l3 @0", "a2");

			Assert.AreEqual(sql.SQL, "l1 @0\nl2 @1\nl3 @2");
			Assert.AreEqual(sql.Arguments.Length, 3);
			Assert.AreEqual(sql.Arguments[0], "a0");
			Assert.AreEqual(sql.Arguments[1], "a1");
			Assert.AreEqual(sql.Arguments[2], "a2");
		}

		[Test]
		public void append_with_args2()
		{
			var sql = new Sql();
			sql.Append("l1");
			sql.Append("l2 @0 @1", "a1", "a2");
			sql.Append("l3 @0", "a3");

			Assert.AreEqual(sql.SQL, "l1\nl2 @0 @1\nl3 @2");
			Assert.AreEqual(sql.Arguments.Length, 3);
			Assert.AreEqual(sql.Arguments[0], "a1");
			Assert.AreEqual(sql.Arguments[1], "a2");
			Assert.AreEqual(sql.Arguments[2], "a3");
		}

		[Test]
		public void invalid_arg_index()
		{
			Assert.Throws<ArgumentOutOfRangeException>(()=>{
				var sql = new Sql();
				sql.Append("arg @0 @1", "a0");
				Assert.AreEqual(sql.SQL, "arg @0 @1");
			});
		}

		[Test]
		public void invalid_arg_name()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				var sql = new Sql();
				sql.Append("arg @name1 @name2", new { x = 1, y = 2 });
				Assert.AreEqual(sql.SQL, "arg @0 @1");
			});
		}

		[Test]
		public void append_instances()
		{
			var sql = new Sql("l0 @0", "a0");
			var sql1 = new Sql("l1 @0", "a1");
			var sql2 = new Sql("l2 @0", "a2");

			Assert.AreSame(sql.Append(sql1), sql);
			Assert.AreSame(sql.Append(sql2), sql);

			Assert.AreEqual(sql.SQL, "l0 @0\nl1 @1\nl2 @2");
			Assert.AreEqual(sql.Arguments.Length, 3);
			Assert.AreEqual(sql.Arguments[0], "a0");
			Assert.AreEqual(sql.Arguments[1], "a1");
			Assert.AreEqual(sql.Arguments[2], "a2");
		}

		[Test]
		public void ConsecutiveWhere()
		{
			var sql = new Sql()
						.Append("SELECT * FROM blah");

			sql.Append("WHERE x");
			sql.Append("WHERE y");

			Assert.AreEqual(sql.SQL, "SELECT * FROM blah\nWHERE x\nAND y");
		}

		[Test]
		public void ConsecutiveOrderBy()
		{
			var sql = new Sql()
						.Append("SELECT * FROM blah");

			sql.Append("ORDER BY x");
			sql.Append("ORDER BY y");

			Assert.AreEqual(sql.SQL, "SELECT * FROM blah\nORDER BY x\n, y");
		}

		[Test]
		public void param_expansion_1()
		{
			// Simple collection parameter expansion
			var sql = Sql.Builder.Append("@0 IN (@1) @2", 20, new int[] { 1, 2, 3 }, 30);
			Assert.AreEqual(sql.SQL, "@0 IN (@1,@2,@3) @4");
			Assert.AreEqual(sql.Arguments.Length, 5);
			Assert.AreEqual(sql.Arguments[0], 20);
			Assert.AreEqual(sql.Arguments[1], 1);
			Assert.AreEqual(sql.Arguments[2], 2);
			Assert.AreEqual(sql.Arguments[3], 3);
			Assert.AreEqual(sql.Arguments[4], 30);
		}

		[Test]
		public void param_expansion_2()
		{
			// Out of order expansion
			var sql = Sql.Builder.Append("IN (@3) (@1)", null, new int[] { 1, 2, 3 }, null, new int[] { 4, 5, 6 });
			Assert.AreEqual(sql.SQL, "IN (@0,@1,@2) (@3,@4,@5)");
			Assert.AreEqual(sql.Arguments.Length, 6);
			Assert.AreEqual(sql.Arguments[0], 4);
			Assert.AreEqual(sql.Arguments[1], 5);
			Assert.AreEqual(sql.Arguments[2], 6);
			Assert.AreEqual(sql.Arguments[3], 1);
			Assert.AreEqual(sql.Arguments[4], 2);
			Assert.AreEqual(sql.Arguments[5], 3);
		}

		[Test]
		public void param_expansion_named()
		{
			// Expand a named parameter
			var sql = Sql.Builder.Append("IN (@numbers)", new { numbers = (new int[] { 1, 2, 3 }) } );
			Assert.AreEqual(sql.SQL, "IN (@0,@1,@2)");
			Assert.AreEqual(sql.Arguments.Length, 3);
			Assert.AreEqual(sql.Arguments[0], 1);
			Assert.AreEqual(sql.Arguments[1], 2);
			Assert.AreEqual(sql.Arguments[2], 3);
		}

		[Test]
		public void join()
		{
			var sql = Sql.Builder
				.Select("*")
				.From("articles")
				.LeftJoin("comments").On("articles.article_id=comments.article_id");
			Assert.AreEqual(sql.SQL, "SELECT *\nFROM articles\nLEFT JOIN comments\nON articles.article_id=comments.article_id");
		}

	}

}

