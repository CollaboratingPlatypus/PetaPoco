// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseExecuteTests : BaseDatabase
    {
        private readonly PocoData _pd;

        protected BaseExecuteTests(DBTestProvider provider)
            : base(provider)
        {
            _pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
        }

        [Fact]
        public void Execute_GivenSqlAndArgumentAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);
            var sql = $"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = @0";

            var beforeCount = CountNotes();
            var result = DB.Execute(sql, 1);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public void Execute_GivenSqlAndArgumentsAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0,@1)", 1, 2);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public void Execute_GivenSqlAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = 1");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public void Execute_GivenSqlAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(1,2)");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public void ExecuteScalar_GivenSql_ReturnShouldBeValid()
        {
            InsertNotes(3);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}").ShouldBe(3);
        }

        [Fact]
        public void ExecuteScalar_GivenSqlAndParameter_ReturnShouldBeValid()
        {
            InsertNotes(4);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                                  $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} <= @0", 2).ShouldBe(2);
        }

        [Fact]
        public void ExecuteScalar_GivenSqlAndParameters_ReturnShouldBeValid()
        {
            InsertNotes(5);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}" +
                                  $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0, @1)", 1, 2).ShouldBe(2);
        }

        private int CountNotes()
        {
            return DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}");
        }

        private void InsertNotes(int numberToInsert)
        {
            for (var i = 0; i < numberToInsert; i++)
            {
                DB.Insert(new Note
                {
                    CreatedOn = new DateTime(1928, 2, 17, 1, 1, 1, DateTimeKind.Utc).AddDays(i),
                    Text = "Note " + i
                });
            }
        }
    }
}