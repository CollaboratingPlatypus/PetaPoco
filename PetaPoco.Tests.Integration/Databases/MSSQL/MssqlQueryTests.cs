// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/02/06</date>

using System.Linq;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    [Collection("MssqlTests")]
    public class MssqlQueryTests : BaseQueryTests
    {
        public MssqlQueryTests()
            : base(new MssqlDBTestProvider())
        {
        }

        [Fact]
        public void Query_ForPocoGivenDbColumPocoOverlapSqlStringAndParameters_ShouldReturnValidPocoCollection()
        {
            DB.Insert(new PocoOverlapPoco1 { Column1 = "A", Column2 = "B" });
            DB.Insert(new PocoOverlapPoco2 { Column1 = "B", Column2 = "A" });

            var sql = @"FROM BugInvestigation_10R9LZYK
                        JOIN BugInvestigation_5TN5C4U4 ON BugInvestigation_10R9LZYK.[ColumnA] = BugInvestigation_5TN5C4U4.[Column2]";

            var poco1 = DB.Query<PocoOverlapPoco1>(sql).ToList().Single();

            sql = @"FROM BugInvestigation_5TN5C4U4
                    JOIN BugInvestigation_10R9LZYK ON BugInvestigation_10R9LZYK.[ColumnA] = BugInvestigation_5TN5C4U4.[Column2]";
            var poco2 = DB.Query<PocoOverlapPoco2>(sql).ToList().Single();

            poco1.Column1.ShouldBe("A");
            poco1.Column2.ShouldBe("B");

            poco2.Column1.ShouldBe("B");
            poco2.Column2.ShouldBe("A");
        }

        [ExplicitColumns]
        [TableName("BugInvestigation_10R9LZYK")]
        public class PocoOverlapPoco1
        {
            [Column("ColumnA")]
            public string Column1 { get; set; }

            [Column]
            public string Column2 { get; set; }
        }

        [ExplicitColumns]
        [TableName("BugInvestigation_5TN5C4U4")]
        public class PocoOverlapPoco2
        {
            [Column("ColumnA")]
            public string Column1 { get; set; }

            [Column]
            public string Column2 { get; set; }
        }
    }
}