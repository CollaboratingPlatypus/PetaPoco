using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using PetaPoco.Providers;
using System.Data.SqlClient;

namespace PetaPoco.Tests.Unit
{
    public class StoredProcTests
    {
        IDatabaseBuildConfiguration _config = DatabaseConfiguration.Build()
                .UsingConnectionString("Data Source=qa.webdb.aiss.st.com,29998;Initial Catalog=AAS_REPORTING;Integrated Security=true;")
                .UsingProvider<SqlServerDatabaseProvider>();

        [Fact]
        public void ReaderNoParams()
        {            
            using (var db = new Database(_config))
            {
                var recs = db.QueryProc<AAS_TRACE>("ReaderNoParams");
                var rec = recs.First();
                rec.TX_ID.ShouldBe(32866827);
            }
        }

        [Fact]
        public void ReaderWithParams()
        {
            using (var db = new Database(_config))
            {
                var recs = db.QueryProc<AAS_TRACE>("ReaderWithParams", new { tx_id = 32866827 });
                recs.Count().ShouldBe(1);
                recs.Single().TX_ID.ShouldBe(32866827);
            }
        }

        [Theory]
        [InlineData("tx_id")]
        [InlineData("@tx_id")]
        public void ReaderWithDbParam(string name)
        {
            using (var db = new Database(_config))
            {
                var parm = new SqlParameter(name, 32866827);
                var recs = db.QueryProc<AAS_TRACE>("ReaderWithParams", parm).ToList();
                recs.Count().ShouldBe(1);
                recs.Single().TX_ID.ShouldBe(32866827);
            }
        }

        [Fact]
        public void Query()
        {
            using (var db = new Database(_config))
            {
                var recs = db.Query<AAS_TRACE>();
                var rec = recs.First();
                rec.TX_ID.ShouldBe(32866827);
            }
        }

        [Fact]
        public void ScalarNoParams()
        {
            using (var db = new Database(_config))
            {
                var count = db.ExecuteScalarProc<int>("ScalarNoParams");
                count.ShouldBe(6574680);
            }
        }

        [Theory]
        [InlineData("subcode")]
        [InlineData("@subcode")]
        public void ScalarWithDbParam(string name)
        {
            using (var db = new Database(_config))
            {
                var parm = new SqlParameter(name, "E8124872");
                var count = db.ExecuteScalarProc<int>("ScalarWithParams", parm);
                count.ShouldBe(149229);
            }
        }

        public class AAS_TRACE
        {
            public int TX_ID { get; set; }
            public int CONTROL_NO { get; set; }
            public string SUBCODE { get; set; }
            public DateTime DATE_ENTERED { get; set; }
        }
    }
}
