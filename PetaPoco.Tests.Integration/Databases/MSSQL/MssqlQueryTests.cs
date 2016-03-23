// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/02/06</date>

using System;
using System.Collections.Generic;
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

            var sql = @"FROM BugInvestigation_64O6LT8U
                        JOIN BugInvestigation_5TN5C4U4 ON BugInvestigation_64O6LT8U.[ColumnA] = BugInvestigation_5TN5C4U4.[Column2]";

            var poco1 = DB.Query<PocoOverlapPoco1>(sql).ToList().Single();

            sql = @"FROM BugInvestigation_5TN5C4U4
                    JOIN BugInvestigation_64O6LT8U ON BugInvestigation_64O6LT8U.[ColumnA] = BugInvestigation_5TN5C4U4.[Column2]";
            var poco2 = DB.Query<PocoOverlapPoco2>(sql).ToList().Single();

            poco1.Column1.ShouldBe("A");
            poco1.Column2.ShouldBe("B");

            poco2.Column1.ShouldBe("B");
            poco2.Column2.ShouldBe("A");
        }

        [ExplicitColumns]
        [TableName("BugInvestigation_64O6LT8U")]
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
        
        #region Multi-result set Tests
        [Fact]
        public void Query_MultiResultsSet_SingleResultsSetSinglePoco__ShouldReturnValidPocoCollection()
        {
            AddPeople(1, 0);
            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var sql = "SET CONCAT_NULL_YIELDS_NULL ON;" +
                      $"SELECT * FROM [{pd.TableInfo.TableName}] WHERE [{pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName}] LIKE @0 + '%'";

            List<Person> result;
            using (var multi = DB.QueryMultiple(sql, "Peta"))
            {
                result = multi.Read<Person>().ToList();
            }

            result.Count.ShouldBe(1);
            var person = result.First();

            person.Id.ShouldNotBe(Guid.Empty);
            person.Name.ShouldStartWith("Peta");
            person.Age.ShouldBe(18);
        }

        [Fact]
        public void Query_MultiResultsSet_SingleResultsSetMultiPoco__ShouldReturnValidPocoCollection()
        {
            AddOrders(1);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = "SET CONCAT_NULL_YIELDS_NULL ON;" +
                      $"SELECT TOP 1 * FROM [{od.TableInfo.TableName}] o INNER JOIN [{pd.TableInfo.TableName}] p ON p.[{pd.Columns.Values.First(p => p.PropertyInfo.Name == "Id").ColumnName}] = o.[{od.Columns.Values.First(p => p.PropertyInfo.Name == "PersonId").ColumnName}] WHERE [{pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName}] = @0 ORDER BY 1 DESC";

            List<Order> result;
            using (var multi = DB.QueryMultiple(sql, "Peta0"))
            {
                result = multi.Read<Order, Person, Order>((o, p) => { o.Person = p; return o; }).ToList();
            }

            result.Count.ShouldBe(1);
            var order = result.First();

            order.PoNumber.ShouldStartWith("PO");
            order.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
            order.PersonId.ShouldNotBe(Guid.Empty);
            order.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            order.CreatedBy.ShouldStartWith("Harry");

            order.Person.ShouldNotBeNull();
            order.Person.Id.ShouldNotBe(Guid.Empty);
            order.Person.Name.ShouldStartWith("Peta");
            order.Person.Age.ShouldBe(18);
        }

        [Fact]
        public void Query_MultiResultsSet_MultiResultSetSinglePoco__ShouldReturnValidPocoCollection()
        {
            AddOrders(1);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SET CONCAT_NULL_YIELDS_NULL ON;SELECT * FROM [{od.TableInfo.TableName}] o WHERE [{od.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName}] = @0;SELECT * FROM [{pd.TableInfo.TableName}] p WHERE [{pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName}] = @1;";

            Order order;
            using (var multi = DB.QueryMultiple(sql, "1", "Peta0"))
            {
                order = multi.Read<Order>().First();
                order.Person = multi.Read<Person>().First();
            }

            order.PoNumber.ShouldStartWith("PO");
            order.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
            order.PersonId.ShouldNotBe(Guid.Empty);
            order.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            order.CreatedBy.ShouldStartWith("Harry");

            order.Person.ShouldNotBeNull();
            order.Person.Id.ShouldNotBe(Guid.Empty);
            order.Person.Name.ShouldStartWith("Peta");
            order.Person.Age.ShouldBe(18);
        }

        [Fact]
        public void Query_MultiResultsSet_MultiResultSetMultiPoco__ShouldReturnValidPocoCollection()
        {
            AddOrders(12);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var old = PocoData.ForType(typeof(OrderLine), DB.DefaultMapper);
            var sql = "SET CONCAT_NULL_YIELDS_NULL ON;" +
                      $"SELECT * FROM [{od.TableInfo.TableName}] o INNER JOIN [{pd.TableInfo.TableName}] p ON p.[{pd.Columns.Values.First(p => p.PropertyInfo.Name == "Id").ColumnName}] = o.[{od.Columns.Values.First(p => p.PropertyInfo.Name == "PersonId").ColumnName}] ORDER BY o.[{od.Columns.Values.First(p => p.PropertyInfo.Name == "Id").ColumnName}] ASC;SELECT * FROM [{old.TableInfo.TableName}] ol ORDER BY ol.[{old.Columns.Values.First(c => c.PropertyInfo.Name == "OrderId").ColumnName}] ASC";

            List<Order> results;
            using (var multi = DB.QueryMultiple(sql))
            {
                results = multi.Read<Order, Person, Order>((o, p) => { o.Person = p; return o; }).ToList();
                var orderLines = multi.Read<OrderLine>().ToList();

                foreach (var order in results)
                {
                    order.OrderLines = orderLines.Where(ol => ol.OrderId == order.Id).ToList();
                }
            }

            results.Count.ShouldBe(12);

            results.ForEach(o =>
            {
                o.PoNumber.ShouldStartWith("PO");
                o.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
                o.PersonId.ShouldNotBe(Guid.Empty);
                o.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                o.CreatedBy.ShouldStartWith("Harry");

                o.Person.ShouldNotBeNull();
                o.Person.Id.ShouldNotBe(Guid.Empty);
                o.Person.Name.ShouldStartWith("Peta");
                o.Person.Age.ShouldBeGreaterThanOrEqualTo(18);

                o.OrderLines.Count.ShouldBe(2);
                var firstOrderLine = o.OrderLines.First();
                firstOrderLine.Quantity.ToString().ShouldBe("1");
                firstOrderLine.SellPrice.ShouldBe(9.99m);
                var secondOrderLine = o.OrderLines.Skip(1).First();
                secondOrderLine.Quantity.ToString().ShouldBe("2");
                secondOrderLine.SellPrice.ShouldBe(19.98m);
            });
        }

        #endregion

    }
}