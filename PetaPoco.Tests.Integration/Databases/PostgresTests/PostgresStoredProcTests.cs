using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    [Collection("Postgres")]
    public class PostgresStoredProcTests : StoredProcTests
    {
        protected override Type DataParameterType => typeof(NpgsqlParameter);

        public PostgresStoredProcTests()
            : base(new PostgresTestProvider())
        {
        }

        private async Task<List<dynamic>> GetDataAsync(int schemaIndex)
        {
            const string cursorName = "p_out_cursor";

            DB.BeginTransaction();
            try
            {
                using (var cmd = (DB as Database).CreateCommand(
                    DB.Connection,
                    CommandType.Text,
                    "CALL SchemaVariantProc(@0, @1)",
                    new NpgsqlParameter()
                    {
                        NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer,
                        Direction = ParameterDirection.Input,
                        Value = schemaIndex
                    },
                    new NpgsqlParameter()
                    {
                        NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor,
                        Direction = ParameterDirection.InputOutput,
                        Value = cursorName
                    }))
                {
                    cmd.ExecuteNonQuery();
                }

                var result = await DB.FetchAsync<dynamic>($";FETCH ALL FROM \"{cursorName}\";");
                DB.CompleteTransaction();

                return result;
            }
            catch
            {
                DB.AbortTransaction();
                throw;
            }
        }

        [Fact]
        public override async Task SchemaVariantProc_WithParam_ShouldReturnCorrectDynamicTypeCollection()
        {
            var expectedFirst = new Dictionary<string, object>
            {
                { "One", 1 },
                { "Two", 2 },
                { "Three", 3 }
            };
            var expectedSecond = new Dictionary<string, object>
            {
                { "Ten", 10 },
                { "Twenty", 20 },
                { "Thirty", 30 }
            };

            IDictionary<string, object> actualFirst = (await GetDataAsync(0)).First();
            foreach (var item in actualFirst)
            {
                expectedFirst.ShouldContainKey(item.Key);
                expectedFirst[item.Key].ShouldBe(item.Value);
            }
            
            IDictionary<string, object> actualSecond = (await GetDataAsync(1)).First();
            foreach (var item in actualSecond)
            {
                expectedSecond.ShouldContainKey(item.Key);
                expectedSecond[item.Key].ShouldBe(item.Value);
            }
        }
    }
}
