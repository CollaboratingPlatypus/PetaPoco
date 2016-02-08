// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/02/06</date>

using System;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
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

        [Fact]
        public void Query_ForPocoGivenSqlString_GivenSqlStartingWithSet__ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = "SET CONCAT_NULL_YIELDS_NULL ON;" +
                      $"SELECT * FROM [{pd.TableInfo.TableName}] WHERE [{pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName}] = @0";

            var results = DB.Query<Order>(sql, OrderStatus.Pending).ToList();
            results.Count.ShouldBe(3);

            results.ForEach(o =>
            {
                o.PoNumber.ShouldStartWith("PO");
                o.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
                o.PersonId.ShouldNotBe(Guid.Empty);
                o.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                o.CreatedBy.ShouldStartWith("Harry");
            });
        }

        [Fact]
        public void Query_ForPocoGivenSqlString_GivenSqlStartingWithDeclare__ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = "DECLARE @@v INT;" +
                      "SET @@v = 1;" +
                      $"SELECT * FROM [{pd.TableInfo.TableName}] WHERE [{pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName}] = @0";

            var results = DB.Query<Order>(sql, OrderStatus.Pending).ToList();
            results.Count.ShouldBe(3);

            results.ForEach(o =>
            {
                o.PoNumber.ShouldStartWith("PO");
                o.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
                o.PersonId.ShouldNotBe(Guid.Empty);
                o.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                o.CreatedBy.ShouldStartWith("Harry");
            });
        }

        [Fact]
        public void Query_ForPocoGivenSqlString_GivenSqlStartingWithWith__ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var columns = string.Join(", ", pd.Columns.Select(c => DB.Provider.EscapeSqlIdentifier(c.Value.ColumnName)));
            var sql = string.Format(@"WITH [{0}_CTE] ({1})
                                      AS
                                      (
                                          SELECT {1} FROM {0}
                                      )
                                      SELECT *
                                      FROM [{0}_CTE]
                                      WHERE [{2}] = @0;", pd.TableInfo.TableName, columns,
                pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName);

            var results = DB.Query<Order>(sql, OrderStatus.Pending).ToList();
            results.Count.ShouldBe(3);

            results.ForEach(o =>
            {
                o.PoNumber.ShouldStartWith("PO");
                o.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
                o.PersonId.ShouldNotBe(Guid.Empty);
                o.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                o.CreatedBy.ShouldStartWith("Harry");
            });
        }
    }
}