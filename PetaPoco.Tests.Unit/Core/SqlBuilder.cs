// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

using System;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    [RequiresCleanUpAttribute]
    public class SqlTests
    {
        [Fact]
        public void simple_append()
        {
            var sql = new Sql();

            sql.Append("LINE 1");
            sql.Append("LINE 2");
            sql.Append("LINE 3");

            sql.SQL.ShouldBe("LINE 1\nLINE 2\nLINE 3");
            sql.Arguments.Length.ShouldBe(0);
        }

        [Fact]
        public void single_arg()
        {
            var sql = new Sql();

            sql.Append("arg @0", "a1");

            sql.SQL.ShouldBe("arg @0");
            sql.Arguments.Length.ShouldBe(1);
            sql.Arguments[0].ShouldBe("a1");
        }

        [Fact]
        public void multiple_args()
        {
            var sql = new Sql();

            sql.Append("arg @0 @1", "a1", "a2");

            sql.SQL.ShouldBe("arg @0 @1");
            sql.Arguments.Length.ShouldBe(2);
            sql.Arguments[0].ShouldBe("a1");
            sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        public void unused_args()
        {
            var sql = new Sql();

            sql.Append("arg @0 @2", "a1", "a2", "a3");

            sql.SQL.ShouldBe("arg @0 @1");
            sql.Arguments.Length.ShouldBe(2);
            sql.Arguments[0].ShouldBe("a1");
            sql.Arguments[1].ShouldBe("a3");
        }

        [Fact]
        public void unordered_args()
        {
            var sql = new Sql();

            sql.Append("arg @2 @1", "a1", "a2", "a3");

            sql.SQL.ShouldBe("arg @0 @1");
            sql.Arguments.Length.ShouldBe(2);
            sql.Arguments[0].ShouldBe("a3");
            sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        public void repeated_args()
        {
            var sql = new Sql();

            sql.Append("arg @0 @1 @0 @1", "a1", "a2");

            sql.SQL.ShouldBe("arg @0 @1 @2 @3");
            sql.Arguments.Length.ShouldBe(4);
            sql.Arguments[0].ShouldBe("a1");
            sql.Arguments[1].ShouldBe("a2");
            sql.Arguments[2].ShouldBe("a1");
            sql.Arguments[3].ShouldBe("a2");
        }

        [Fact]
        public void mysql_user_vars()
        {
            var sql = new Sql();

            sql.Append("arg @@user1 @2 @1 @@@system1", "a1", "a2", "a3");

            sql.SQL.ShouldBe("arg @@user1 @0 @1 @@@system1");
            sql.Arguments.Length.ShouldBe(2);
            sql.Arguments[0].ShouldBe("a3");
            sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        public void named_args()
        {
            var sql = new Sql();

            sql.Append("arg @name @password", new {name = "n", password = "p"});

            sql.SQL.ShouldBe("arg @0 @1");
            sql.Arguments.Length.ShouldBe(2);
            sql.Arguments[0].ShouldBe("n");
            sql.Arguments[1].ShouldBe("p");
        }

        [Fact]
        public void mixed_named_and_numbered_args()
        {
            var sql = new Sql();

            sql.Append("arg @0 @name @1 @password @2", "a1", "a2", "a3", new {name = "n", password = "p"});

            sql.SQL.ShouldBe("arg @0 @1 @2 @3 @4");
            sql.Arguments.Length.ShouldBe(5);
            sql.Arguments[0].ShouldBe("a1");
            sql.Arguments[1].ShouldBe("n");
            sql.Arguments[2].ShouldBe("a2");
            sql.Arguments[3].ShouldBe("p");
            sql.Arguments[4].ShouldBe("a3");
        }

        [Fact]
        public void append_with_args()
        {
            var sql = new Sql();

            sql.Append("l1 @0", "a0");
            sql.Append("l2 @0", "a1");
            sql.Append("l3 @0", "a2");

            sql.SQL.ShouldBe("l1 @0\nl2 @1\nl3 @2");
            sql.Arguments.Length.ShouldBe(3);
            sql.Arguments[0].ShouldBe("a0");
            sql.Arguments[1].ShouldBe("a1");
            sql.Arguments[2].ShouldBe("a2");
        }

        [Fact]
        public void append_with_args2()
        {
            var sql = new Sql();

            sql.Append("l1");
            sql.Append("l2 @0 @1", "a1", "a2");
            sql.Append("l3 @0", "a3");

            sql.SQL.ShouldBe("l1\nl2 @0 @1\nl3 @2");
            sql.Arguments.Length.ShouldBe(3);
            sql.Arguments[0].ShouldBe("a1");
            sql.Arguments[1].ShouldBe("a2");
            sql.Arguments[2].ShouldBe("a3");
        }

        [Fact]
        public void invalid_arg_index()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                var sql = new Sql();
                sql.Append("arg @0 @1", "a0");
                sql.SQL.ShouldBe("arg @0 @1");
            });
        }

        [Fact]
        public void invalid_arg_name()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var sql = new Sql();
                sql.Append("arg @name1 @name2", new {x = 1, y = 2});
                sql.SQL.ShouldBe("arg @0 @1");
            });
        }

        [Fact]
        public void append_instances()
        {
            var sql = new Sql("l0 @0", "a0");
            var sql1 = new Sql("l1 @0", "a1");
            var sql2 = new Sql("l2 @0", "a2");

            sql.Append(sql1).ShouldBe(sql);
            sql.Append(sql2).ShouldBe(sql);

            sql.SQL.ShouldBe("l0 @0\nl1 @1\nl2 @2");
            sql.Arguments.Length.ShouldBe(3);
            sql.Arguments[0].ShouldBe("a0");
            sql.Arguments[1].ShouldBe("a1");
            sql.Arguments[2].ShouldBe("a2");
        }

        [Fact]
        public void ConsecutiveWhere()
        {
            var sql = new Sql()
                .Append("SELECT * FROM blah");

            sql.Append("WHERE x");
            sql.Append("WHERE y");

            sql.SQL.ShouldBe("SELECT * FROM blah\nWHERE x\nAND y");
        }

        [Fact]
        public void ConsecutiveOrderBy()
        {
            var sql = new Sql()
                .Append("SELECT * FROM blah");

            sql.Append("ORDER BY x");
            sql.Append("ORDER BY y");

            sql.SQL.ShouldBe("SELECT * FROM blah\nORDER BY x\n, y");
        }

        [Fact]
        public void param_expansion_1()
        {
            // Simple collection parameter expansion
            var sql = Sql.Builder.Append("@0 IN (@1) @2", 20, new int[] {1, 2, 3}, 30);

            sql.SQL.ShouldBe("@0 IN (@1,@2,@3) @4");
            sql.Arguments.Length.ShouldBe(5);
            sql.Arguments[0].ShouldBe(20);
            sql.Arguments[1].ShouldBe(1);
            sql.Arguments[2].ShouldBe(2);
            sql.Arguments[3].ShouldBe(3);
            sql.Arguments[4].ShouldBe(30);
        }

        [Fact]
        public void param_expansion_2()
        {
            // Out of order expansion
            var sql = Sql.Builder.Append("IN (@3) (@1)", null, new int[] {1, 2, 3}, null, new int[] {4, 5, 6});

            sql.SQL.ShouldBe("IN (@0,@1,@2) (@3,@4,@5)");
            sql.Arguments.Length.ShouldBe(6);
            sql.Arguments[0].ShouldBe(4);
            sql.Arguments[1].ShouldBe(5);
            sql.Arguments[2].ShouldBe(6);
            sql.Arguments[3].ShouldBe(1);
            sql.Arguments[4].ShouldBe(2);
            sql.Arguments[5].ShouldBe(3);
        }

        [Fact]
        public void param_expansion_named()
        {
            // Expand a named parameter
            var sql = Sql.Builder.Append("IN (@numbers)", new {numbers = (new int[] {1, 2, 3})});

            sql.SQL.ShouldBe("IN (@0,@1,@2)");
            sql.Arguments.Length.ShouldBe(3);
            sql.Arguments[0].ShouldBe(1);
            sql.Arguments[1].ShouldBe(2);
            sql.Arguments[2].ShouldBe(3);
        }

        [Fact]
        public void join()
        {
            var sql = Sql.Builder
                .Select("*")
                .From("articles")
                .LeftJoin("comments").On("articles.article_id=comments.article_id");

            sql.SQL.ShouldBe("SELECT *\nFROM articles\nLEFT JOIN comments\nON articles.article_id=comments.article_id");
        }
    }
}