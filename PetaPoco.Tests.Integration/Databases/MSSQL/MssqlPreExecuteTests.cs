using PetaPoco.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("Mssql")]
    public class MssqlPreExecuteTests : BaseDatabase
    {
        public MsssqlPreExecuteDatabaseProvider Provider => DB.Provider as MsssqlPreExecuteDatabaseProvider;

        public MssqlPreExecuteTests() : base(new MssqlPreExecuteDBTestProvider())
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


        public class MssqlPreExecuteDBTestProvider : MssqlDBTestProvider
        {
            protected override IDatabase LoadFromConnectionName(string name)
            {
                var config = BuildFromConnectionName(name);
                config.UsingProvider<MsssqlPreExecuteDatabaseProvider>();
                return config.Create();
            }
        }

        public class MsssqlPreExecuteDatabaseProvider : SqlServerDatabaseProvider
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
