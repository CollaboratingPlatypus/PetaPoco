using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleQueryTests : QueryTests
    {
        protected OracleQueryTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleQueryTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleQueryTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }
        }

        [Fact]
        public override async Task FetchAsyncWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);

            var sql = new Sql(
                $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0", OrderStatus.Pending);

            var results = await DB.FetchAsync<dynamic>(2, 1, sql);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override async Task FetchAsyncWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = await DB.FetchAsync<dynamic>(2, 1, sql, OrderStatus.Pending);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override void FetchWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql(
                $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0", OrderStatus.Pending);

            var results = DB.Fetch<dynamic>(2, 1, sql);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override void FetchWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                      $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Fetch<dynamic>(2, 1, sql, OrderStatus.Pending);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override void SkipAndTake_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql(
                $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0", OrderStatus.Pending);

            var results = DB.SkipTake<dynamic>(2, 1, sql);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override void SkipAndTake_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                      $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.SkipTake<dynamic>(2, 1, sql, OrderStatus.Pending);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override async Task SkipAndTakeAsync_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql(
                $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0", OrderStatus.Pending);

            var results = await DB.SkipTakeAsync<dynamic>(2, 1, sql);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override async Task SkipAndTakeAsync_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT t.* FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} t " +
                $"WHERE t.{DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = await DB.SkipTakeAsync<dynamic>(2, 1, sql, OrderStatus.Pending);
            results.Count.ShouldBe(1);
        }

        [Fact]
        public override void QueryMultiple_ForSingleResultsSetWithSinglePoco_ShouldReturnValidPocoCollection()
        {
            AddPeople(1, 0);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;

            var sql = $@"SELECT *
    FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}
    WHERE {DB.Provider.EscapeSqlIdentifier(pdName)} LIKE @0 || '%'";

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
        public override void QueryMultiple_ForSingleResultsSetWithMultiPoco_ShouldReturnValidPocoCollection()
        {
            AddOrders(1);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var pdId = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;
            var odPersonId = od.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName;

            //Oracle 12c and above only
    //        var sql = $@"SELECT *
    //FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
    //    INNER JOIN {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p ON p.{DB.Provider.EscapeSqlIdentifier(pdId)} = o.{DB.Provider.EscapeSqlIdentifier(odPersonId)}
    //WHERE p.{DB.Provider.EscapeSqlIdentifier(pdName)} = @0
    //ORDER BY 1 DESC
    //FETCH FIRST 1 ROWS ONLY";

            var sql = $@"SELECT *
    FROM (SELECT *
        FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
            INNER JOIN {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p
                ON p.{DB.Provider.EscapeSqlIdentifier(pdId)} = o.{DB.Provider.EscapeSqlIdentifier(odPersonId)}
        WHERE p.{DB.Provider.EscapeSqlIdentifier(pdName)} = @0
        ORDER BY 1 DESC)
    WHERE ROWNUM <= 1";

            List<Order> result;
            using (var multi = DB.QueryMultiple(sql, "Peta0"))
            {
                result = multi.Read<Order, Person, Order>((o, p) =>
                {
                    o.Person = p;
                    return o;
                }).ToList();
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

        // FIXME: Oracle.ManagedDataAccess.Client.OracleException : ORA-03048: SQL reserved word ';' is not syntactically valid following '...ORDER BY o.Id ASC'
        [Fact(Skip = "Limited support for QueryMultiple by provider due to need for multiple statements in a single command.")]
        public override void QueryMultiple_ForMultiResultsSetWithSinglePoco_ShouldReturnValidPocoCollection()
        {
            AddOrders(1);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;
            var odId = od.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;

            var sql = $@"SELECT * FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
                         WHERE o.{DB.Provider.EscapeSqlIdentifier(odId)} = @0;
                         SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p
                         WHERE p.{DB.Provider.EscapeSqlIdentifier(pdName)} = @1;";

            Order order;
            using (var multi = DB.QueryMultiple(sql, 1, "Peta0"))
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

        // FIXME: Oracle.ManagedDataAccess.Client.OracleException : ORA-03048: SQL reserved word ';' is not syntactically valid following '...ORDER BY o.Id ASC'
        [Fact(Skip = "Limited support for QueryMultiple by provider due to need for multiple statements in a single command.")]
        public override void QueryMultiple_ForMultiResultsSetWithMultiPoco_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var old = PocoData.ForType(typeof(OrderLine), DB.DefaultMapper);
            var pdId = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var odId = od.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var odPersonId = od.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName;
            var oldOrderId = old.Columns.Values.First(c => c.PropertyInfo.Name == "OrderId").ColumnName;

            var sql = $@"SELECT * FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
                         INNER JOIN {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p ON p.{DB.Provider.EscapeSqlIdentifier(pdId)} = o.{DB.Provider.EscapeSqlIdentifier(odPersonId)}
                         ORDER BY o.{DB.Provider.EscapeSqlIdentifier(odId)} ASC;
                         SELECT * FROM {DB.Provider.EscapeTableName(old.TableInfo.TableName)} ol
                         ORDER BY ol.{DB.Provider.EscapeSqlIdentifier(oldOrderId)} ASC;";

            List<Order> results;
            using (var multi = DB.QueryMultiple(sql))
            {
                results = multi.Read<Order, Person, Order>((o, p) =>
                {
                    o.Person = p;
                    return o;
                }).ToList();

                var orderLines = multi.Read<OrderLine>().ToList();
                foreach (var order in results)
                    order.OrderLines = orderLines.Where(ol => ol.OrderId == order.Id).ToList();
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
    }
}
