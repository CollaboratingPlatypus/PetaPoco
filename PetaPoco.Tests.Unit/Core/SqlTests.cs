// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/17</date>

using System;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    [RequiresCleanUp]
    public class SqlTests
    {
        private Sql _sql;

        public SqlTests()
        {
            _sql = new Sql();
        }

        [Fact]
        public void Append_GivenSimpleStrings_ShouldBeValid()
        {
            _sql.Append("LINE 1");
            _sql.Append("LINE 2");
            _sql.Append("LINE 3");

            _sql.SQL.ShouldBe("LINE 1\nLINE 2\nLINE 3");
            _sql.Arguments.Length.ShouldBe(0);
        }

        [Fact]
        public void Append_GivenSignleArgument_ShouldBeValid()
        {
            _sql.Append("arg @0", "a1");

            _sql.SQL.ShouldBe("arg @0");
            _sql.Arguments.Length.ShouldBe(1);
            _sql.Arguments[0].ShouldBe("a1");
        }

        [Fact]
        public void Append_GivenMultipleArguments_ShouldBeValid()
        {
            _sql.Append("arg @0 @1", "a1", "a2");

            _sql.SQL.ShouldBe("arg @0 @1");
            _sql.Arguments.Length.ShouldBe(2);
            _sql.Arguments[0].ShouldBe("a1");
            _sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        [Description("Question: should this not throw?")]
        public void Append_GivenUnusedArguments_ShouldBeValid()
        {
            _sql.Append("arg @0 @2", "a1", "a2", "a3");

            _sql.SQL.ShouldBe("arg @0 @1");
            _sql.Arguments.Length.ShouldBe(2);
            _sql.Arguments[0].ShouldBe("a1");
            _sql.Arguments[1].ShouldBe("a3");
        }

        [Fact]
        public void Append_GivenUnorderedArguments_ShouldBeValid()
        {
            _sql.Append("arg @2 @1", "a1", "a2", "a3");

            _sql.SQL.ShouldBe("arg @0 @1");
            _sql.Arguments.Length.ShouldBe(2);
            _sql.Arguments[0].ShouldBe("a3");
            _sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        public void Append_GivenRepeatedArguments_ShouldBeValid()
        {
            _sql.Append("arg @0 @1 @0 @1", "a1", "a2");

            _sql.SQL.ShouldBe("arg @0 @1 @2 @3");
            _sql.Arguments.Length.ShouldBe(4);
            _sql.Arguments[0].ShouldBe("a1");
            _sql.Arguments[1].ShouldBe("a2");
            _sql.Arguments[2].ShouldBe("a1");
            _sql.Arguments[3].ShouldBe("a2");
        }

        [Fact]
        public void Append_GivenMySqlUserVariables_ShouldBeValid()
        {
            _sql.Append("arg @@user1 @2 @1 @@@system1", "a1", "a2", "a3");

            _sql.SQL.ShouldBe("arg @@user1 @0 @1 @@@system1");
            _sql.Arguments.Length.ShouldBe(2);
            _sql.Arguments[0].ShouldBe("a3");
            _sql.Arguments[1].ShouldBe("a2");
        }

        [Fact]
        public void Append_GivenNameArguments_ShouldBeValid()
        {
            _sql.Append("arg @name @password", new { name = "n", password = "p" });

            _sql.SQL.ShouldBe("arg @0 @1");
            _sql.Arguments.Length.ShouldBe(2);
            _sql.Arguments[0].ShouldBe("n");
            _sql.Arguments[1].ShouldBe("p");
        }

        [Fact]
        public void Append_GivenMixedNameAndNumberArguments_ShouldBeValid()
        {
            _sql.Append("arg @0 @name @1 @password @2", "a1", "a2", "a3", new { name = "n", password = "p" });

            _sql.SQL.ShouldBe("arg @0 @1 @2 @3 @4");
            _sql.Arguments.Length.ShouldBe(5);
            _sql.Arguments[0].ShouldBe("a1");
            _sql.Arguments[1].ShouldBe("n");
            _sql.Arguments[2].ShouldBe("a2");
            _sql.Arguments[3].ShouldBe("p");
            _sql.Arguments[4].ShouldBe("a3");
        }

        [Fact]
        public void Append_GivenConsecutiveArguments_ShouldBeValid()
        {
            _sql.Append("l1 @0", "a0");
            _sql.Append("l2 @0", "a1");
            _sql.Append("l3 @0", "a2");

            _sql.SQL.ShouldBe("l1 @0\nl2 @1\nl3 @2");
            _sql.Arguments.Length.ShouldBe(3);
            _sql.Arguments[0].ShouldBe("a0");
            _sql.Arguments[1].ShouldBe("a1");
            _sql.Arguments[2].ShouldBe("a2");
        }

        [Fact]
        public void Append_GivenConsecutiveComplexArguments_ShouldBeValid()
        {
            _sql.Append("l1");
            _sql.Append("l2 @0 @1", "a1", "a2");
            _sql.Append("l3 @0", "a3");

            _sql.SQL.ShouldBe("l1\nl2 @0 @1\nl3 @2");
            _sql.Arguments.Length.ShouldBe(3);
            _sql.Arguments[0].ShouldBe("a1");
            _sql.Arguments[1].ShouldBe("a2");
            _sql.Arguments[2].ShouldBe("a3");
        }

        [Fact]
        public void Append_GivenInvalidNumberOfArguments_ShouldThrow()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                _sql.Append("arg @0 @1", "a0");
                _sql.SQL.ShouldBe("arg @0 @1");
            });
        }

        [Fact]
        public void Append_GivenInvalidArgumentNames_ShouldThrow()
        {
            Should.Throw<ArgumentException>(() =>
            {
                _sql.Append("arg @name1 @name2", new { x = 1, y = 2 });
                _sql.SQL.ShouldBe("arg @0 @1");
            });
        }

        [Fact]
        public void Append_GivenSqLInstance_ShouldBeValid()
        {
            _sql = new Sql("l0 @0", "a0");
            var sql1 = new Sql("l1 @0", "a1");
            var sql2 = new Sql("l2 @0", "a2");

            _sql.Append(sql1).ShouldBe(_sql);
            _sql.Append(sql2).ShouldBe(_sql);

            _sql.SQL.ShouldBe("l0 @0\nl1 @1\nl2 @2");
            _sql.Arguments.Length.ShouldBe(3);
            _sql.Arguments[0].ShouldBe("a0");
            _sql.Arguments[1].ShouldBe("a1");
            _sql.Arguments[2].ShouldBe("a2");
        }

        [Fact]
        public void Append_GivenConsecutiveSets_ShouldBeValid()
        {
            _sql = new Sql()
                .Append("UPDATE blah");

            _sql.Append("SET a = 1");
            _sql.Append("SET b = 2");

            _sql.SQL.ShouldBe("UPDATE blah\nSET a = 1\n, b = 2");
        }

        [Fact]
        public void Set_GivenConsecutiveSets_ShouldBeValid()
        {
            _sql = new Sql()
                .Append("UPDATE blah");

            _sql.Set("a = 1");
            _sql.Set("b = 2");

            _sql.SQL.ShouldBe("UPDATE blah\nSET a = 1\n, b = 2");
        }

        [Fact]
        public void Append_GivenConsecutiveSetsAndWheres_ShouldBeValid()
        {
            _sql = new Sql()
                .Append("UPDATE blah");

            _sql.Append("SET a = 1");
            _sql.Append("SET b = 2");
            _sql.Append("WHERE x");
            _sql.Append("WHERE y");

            _sql.SQL.ShouldBe("UPDATE blah\nSET a = 1\n, b = 2\nWHERE x\nAND y");
        }

        [Fact]
        public void Append_GivenConsecutiveWheres_ShouldBeValid()
        {
            _sql = new Sql()
                .Append("SELECT * FROM blah");

            _sql.Append("WHERE x");
            _sql.Append("WHERE y");

            _sql.SQL.ShouldBe("SELECT * FROM blah\nWHERE x\nAND y");
        }

        [Fact]
        public void Append_GivenConsecutiveOrderBys_ShouldBeValid()
        {
            _sql = new Sql()
                .Append("SELECT * FROM blah");

            _sql.Append("ORDER BY x");
            _sql.Append("ORDER BY y");

            _sql.SQL.ShouldBe("SELECT * FROM blah\nORDER BY x\n, y");
        }

        [Fact]
        public void Append_GivenArrayAndValue_ShouldBeValid()
        {
            // Simple collection parameter expansion
            _sql = Sql.Builder.Append("@0 IN (@1) @2", 20, new int[] { 1, 2, 3 }, 30);

            _sql.SQL.ShouldBe("@0 IN (@1,@2,@3) @4");
            _sql.Arguments.Length.ShouldBe(5);
            _sql.Arguments[0].ShouldBe(20);
            _sql.Arguments[1].ShouldBe(1);
            _sql.Arguments[2].ShouldBe(2);
            _sql.Arguments[3].ShouldBe(3);
            _sql.Arguments[4].ShouldBe(30);
        }

        [Fact]
        public void Append_GivenTwoArrays_ShouldBeValid()
        {
            // Out of order expansion
            _sql = Sql.Builder.Append("IN (@3) (@1)", null, new[] { 1, 2, 3 }, null, new[] { 4, 5, 6 });

            _sql.SQL.ShouldBe("IN (@0,@1,@2) (@3,@4,@5)");
            _sql.Arguments.Length.ShouldBe(6);
            _sql.Arguments[0].ShouldBe(4);
            _sql.Arguments[1].ShouldBe(5);
            _sql.Arguments[2].ShouldBe(6);
            _sql.Arguments[3].ShouldBe(1);
            _sql.Arguments[4].ShouldBe(2);
            _sql.Arguments[5].ShouldBe(3);
        }

        [Fact]
        public void Append_GivenArray_ShouldBeValid()
        {
            _sql = Sql.Builder.Append("IN (@numbers)", new { numbers = (new[] { 1, 2, 3 }) });

            _sql.SQL.ShouldBe("IN (@0,@1,@2)");
            _sql.Arguments.Length.ShouldBe(3);
            _sql.Arguments[0].ShouldBe(1);
            _sql.Arguments[1].ShouldBe(2);
            _sql.Arguments[2].ShouldBe(3);
        }

        [Fact]
        public void Join_GivenTable_ShouldBeValid()
        {
            _sql = Sql.Builder
                .Select("*")
                .From("articles")
                .LeftJoin("comments").On("articles.article_id=comments.article_id");

            _sql.SQL.ShouldBe("SELECT *\nFROM articles\nLEFT JOIN comments\nON articles.article_id=comments.article_id");
        }

        [Fact]
        [Description("Investigation of reported bug #123")]
        public void Append_GivenMultipleAppends_ShouldBeValid()
        {
            var resource = new
            {
                ResourceName = "p1",
                ResourceDescription = "p2",
                ResourceContent = "p3",
                ResourceData = "p4",
                ResourceGUID = Guid.Parse("C32B630F-FCFE-49FF-A27C-2E4105D4003E"),
                LaunchPath = "p5",
                ResourceType = OrderStatus.Deleted,
                ContentType = "p5",
                SchoolID = "p5",
                DistrictID = "p5",
                UpdatedBy = 87,
                UpdatedDate = new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                IsActive = true,
                Extension = "p9",
                ResourceID = 99,
            };

            _sql.Append("UPDATE [Resource] SET ")
                .Append("[ResourceName] = @0", resource.ResourceName)
                .Append(",[ResourceDescription] = @0", resource.ResourceDescription)
                .Append(",[ResourceContent] = @0", resource.ResourceContent)
                .Append(",[ResourceData] = @0", resource.ResourceData)
                .Append(",[ResourceGUID] = @0", resource.ResourceGUID)
                .Append(",[LaunchPath] = @0", resource.LaunchPath)
                .Append(",[ResourceType] = @0", (int) resource.ResourceType)
                .Append(",[ContentType] = @0", resource.ContentType)
                .Append(",[SchoolID] = @0", resource.SchoolID)
                .Append(",[DistrictID] = @0", resource.DistrictID)
                .Append(",[IsActive] = @0", resource.IsActive)
                .Append(",[UpdatedBy] = @0", resource.UpdatedBy)
                .Append(",[UpdatedDate] = @0", resource.UpdatedDate)
                .Append(",[Extension] = @0", resource.Extension).Append(" WHERE ResourceID=@0", resource.ResourceID);

            _sql.SQL.Replace("\n", "")
                .Replace("\r", "")
                .ShouldBe(
                    @"UPDATE [Resource] SET [ResourceName] = @0,[ResourceDescription] = @1,[ResourceContent] = @2,[ResourceData] = @3,[ResourceGUID] = @4,[LaunchPath] = @5,[ResourceType] = @6,[ContentType] = @7,[SchoolID] = @8,[DistrictID] = @9,[IsActive] = @10,[UpdatedBy] = @11,[UpdatedDate] = @12,[Extension] = @13 WHERE ResourceID=@14");
        }
    }
}