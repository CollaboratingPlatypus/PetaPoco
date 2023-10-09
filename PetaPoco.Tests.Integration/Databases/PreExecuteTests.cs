using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration
{
    public abstract class PreExecuteTests : BaseDbContext
    {
        protected abstract IPreExecuteDatabaseProvider Provider { get; }

        protected PreExecuteTests(TestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public virtual void Execute_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            void act() => DB.Execute(sql);

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual async Task ExecuteAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.ExecuteAsync(sql);

            await Should.ThrowAsync<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual void ExecuteScalar_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("count(*)")
                .From("sometable")
                .Where("foo = @0", expected);

            void act() => DB.ExecuteScalar<int>(sql);

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual async Task ExecuteScalarAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("count(*)")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.ExecuteScalarAsync<int>(sql);

            await Should.ThrowAsync<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual void Insert_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            void act() => DB.Insert("sometable", new { Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual async Task InsertAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            async Task act() => await DB.InsertAsync("sometable", new { Foo = expected });

            await Should.ThrowAsync<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual void Query_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            void act() => DB.Query<string>(sql).First();

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual async Task QueryAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.QueryAsync<string>(sql);

            await Should.ThrowAsync<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual void Update_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            void act() => DB.Update("sometable", "id", new { ID = 3, Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(2);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public virtual async Task UpdateAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            async Task act() => await DB.UpdateAsync("sometable", "id", new { ID = 3, Foo = expected });

            await Should.ThrowAsync<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(2);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        public interface IPreExecuteDatabaseProvider
        {
            bool ThrowExceptions { get; set; }
            List<IDataParameter> Parameters { get; set; }

            void PreExecute(IDbCommand cmd);
        }

        public class PreExecuteException : Exception { }
    }
}
