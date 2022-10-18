using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQLMsData
{
    [Collection("MssqlMsData")]
    [Trait("Category", "MssqlMsData")]
    public class MssqlMsDataPreExecuteTests : BaseDatabase
    {
        public MssqlMsDataPreExecuteDatabaseProvider Provider => DB.Provider as MssqlMsDataPreExecuteDatabaseProvider;

        public MssqlMsDataPreExecuteTests() : base(new MssqlMsDataPreExecuteDBTestProvider())
        {
            Provider.ThrowExceptions = true;
        }


        [Fact]
        public void Query_Calls_PreExecute()
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
        public void Execute_Calls_PreExecute()
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
        public void Insert_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            void act() => DB.Insert("sometable", new { Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void Update_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            void act() => DB.Update("sometable", "id", new { ID = 3, Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(2);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void ExecuteScalar_Calls_PreExecute()
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
        public void QueryAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.QueryAsync<string>(sql).Result.ReadAsync();

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void ExecuteAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("*")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.ExecuteAsync(sql);

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void InsertAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            async Task act() => await DB.InsertAsync("sometable", new { Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void UpdateAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();

            async Task act() => await DB.UpdateAsync("sometable", "id", new { ID = 3, Foo = expected });

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(2);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }

        [Fact]
        public void ExecuteScalarAsync_Calls_PreExecute()
        {
            var expected = Guid.NewGuid().ToString();
            var sql = Sql.Builder
                .Select("count(*)")
                .From("sometable")
                .Where("foo = @0", expected);

            async Task act() => await DB.ExecuteScalarAsync<int>(sql);

            Should.Throw<PreExecuteException>(act);
            Provider.Parameters.Count().ShouldBe(1);
            Provider.Parameters.First().Value.ShouldBe(expected);
        }


        public class MssqlMsDataPreExecuteDBTestProvider : MssqlMsDataDBTestProvider
        {
            protected override IDatabase LoadFromConnectionName(string name)
            {
                var config = BuildFromConnectionName(name);
                config.UsingProvider<MssqlMsDataPreExecuteDatabaseProvider>();
                return config.Create();
            }
        }

        public class MssqlMsDataPreExecuteDatabaseProvider : SqlServerDatabaseProvider
        {
            public bool ThrowExceptions { get; set; }
            public List<IDataParameter> Parameters { get; set; } = new List<IDataParameter>();

            public override void PreExecute(IDbCommand cmd)
            {
                Parameters.Clear();

                if (ThrowExceptions)
                {
                    Parameters = cmd.Parameters.Cast<IDataParameter>().ToList();
                    throw new PreExecuteException();
                }
            }
        }

        public class PreExecuteException : Exception { }
    }


}
