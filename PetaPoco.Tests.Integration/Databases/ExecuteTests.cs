using System;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration
{
    public abstract class ExecuteTests : BaseDbContext
    {
        // TODO: Make this a protected accessor from the base test class
        private readonly PocoData _pd;

        protected ExecuteTests(TestProvider provider)
            : base(provider)
        {
            _pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
        }

        #region Test Helpers

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

        #endregion

        [Fact]
        public virtual void Execute_GivenSqlAndParameterAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = @0", 1);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public virtual void Execute_GivenSqlAndParametersAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0,@1)", 1, 2);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public virtual void Execute_GivenSqlAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = 1");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public virtual void Execute_GivenSqlAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = DB.Execute($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                    $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(1,2)");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public virtual async Task ExecuteAsync_GivenSqlAndParameterAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = await DB.ExecuteAsync($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                               $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = @0", 1);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public virtual async Task ExecuteAsync_GivenSqlAndParametersAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = await DB.ExecuteAsync($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                               $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0,@1)", 1, 2);
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public virtual async Task ExecuteAsync_GivenSqlAffectsOneRow_ShouldReturnOne()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = await DB.ExecuteAsync($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                               $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} = 1");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(1);
            afterCount.ShouldBe(4);
        }

        [Fact]
        public virtual async Task ExecuteAsync_GivenSqlAffectsTwoRows_ShouldReturnTwo()
        {
            InsertNotes(5);

            var beforeCount = CountNotes();
            var result = await DB.ExecuteAsync($"DELETE FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                               $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(1,2)");
            var afterCount = CountNotes();

            beforeCount.ShouldBe(5);
            result.ShouldBe(2);
            afterCount.ShouldBe(3);
        }

        [Fact]
        public virtual void ExecuteScalar_GivenSql_ReturnShouldBeValid()
        {
            InsertNotes(3);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}").ShouldBe(3);
        }

        [Fact]
        public virtual void ExecuteScalar_GivenSqlAndParameter_ReturnShouldBeValid()
        {
            InsertNotes(4);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                  $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} <= @0", 2)
                                  .ShouldBe(2);
        }

        [Fact]
        public virtual void ExecuteScalar_GivenSqlAndParameters_ReturnShouldBeValid()
        {
            InsertNotes(5);

            DB.ExecuteScalar<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                  $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0, @1)", 1, 2)
                                  .ShouldBe(2);
        }

        [Fact]
        public virtual async Task ExecuteScalarAsync_GivenSql_ReturnShouldBeValid()
        {
            InsertNotes(3);

            (await DB.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)}")).ShouldBe(3);
        }

        [Fact]
        public virtual async Task ExecuteScalarAsync_GivenSqlAndParameter_ReturnShouldBeValid()
        {
            InsertNotes(4);

            (await DB.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                              $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} <= @0", 2))
                                              .ShouldBe(2);
        }

        [Fact]
        public virtual async Task ExecuteScalarAsync_GivenSqlAndParameters_ReturnShouldBeValid()
        {
            InsertNotes(5);

            (await DB.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {DB.Provider.EscapeTableName(_pd.TableInfo.TableName)} " +
                                              $"WHERE {DB.Provider.EscapeSqlIdentifier(_pd.TableInfo.PrimaryKey)} IN(@0, @1)", 1, 2))
                                              .ShouldBe(2);
        }
    }
}
