using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using PetaPoco.Tests.Integration.Models.MSAccess;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessQueryTests : QueryTests
    {
        public MSAccessQueryTests()
            : base(new MSAccessTestProvider())
        {
        }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void PageCount_GivenSqlWithGroupBy_ShouldReturnEntireQueryCount() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlStringWithEscapedOrderByColumn_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        // TODO: Async version of Page_ForPocoGivenSqlStringWithEscapedOrderByColumn_ShouldReturnValidPocoCollection

        [Fact]
        public override void Query_ForMultiPocoWithWildcard_ShouldReturnValidPocoCollectionAfterColumnAdded()
        {
            AddJoinableOrders(1);

            var pdOrder = PocoData.ForType(typeof(JoinableOrder), DB.DefaultMapper);
            var pdPerson = PocoData.ForType(typeof(JoinablePerson), DB.DefaultMapper);

            var orderTable = DB.Provider.EscapeTableName(pdOrder.TableInfo.TableName);
            var personTable = DB.Provider.EscapeTableName(pdPerson.TableInfo.TableName);

            var oPersonId = DB.Provider.EscapeSqlIdentifier(pdOrder.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinableOrder.JoinablePersonId)).ColumnName);
            var pId = DB.Provider.EscapeSqlIdentifier(pdPerson.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinablePerson.Id)).ColumnName);
            var randColumn = DB.Provider.EscapeSqlIdentifier("SomeRandomColumn");

            var testQuery = $"SELECT * FROM {orderTable} o INNER JOIN {personTable} p ON o.{oPersonId} = p.{pId}";

            var results = DB.Query<JoinableOrder, JoinablePerson>(testQuery).ToList();
            results.ShouldNotBeEmpty();

            var execStmt = $"ALTER TABLE {orderTable} ADD {randColumn} INT NULL";
            DB.Execute(execStmt);

            results = DB.Query<JoinableOrder, JoinablePerson>(testQuery).ToList();
            results.ShouldNotBeEmpty();
        }

        [Fact]
        public override void Query_ForMultiPocoWithPropertyMissingSetMethod_ShouldThrow()
        {
            AddJoinableOrders(1);

            var pdOrder = PocoData.ForType(typeof(JoinableOrder), DB.DefaultMapper);
            var pdPerson = PocoData.ForType(typeof(JoinablePerson), DB.DefaultMapper);

            var dbOrders = DB.Provider.EscapeTableName(pdOrder.TableInfo.TableName);
            var dbPeople = DB.Provider.EscapeTableName(pdPerson.TableInfo.TableName);

            var oId = DB.Provider.EscapeSqlIdentifier(pdOrder.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinableOrder.Id)).ColumnName);
            var pId = DB.Provider.EscapeSqlIdentifier(pdPerson.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinablePerson.Id)).ColumnName);

            var oPersonId = DB.Provider.EscapeSqlIdentifier(pdOrder.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinableOrder.JoinablePersonId)).ColumnName);
            var pFullName = DB.Provider.EscapeSqlIdentifier(pdPerson.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(JoinablePerson.Name)).ColumnName);

            var sql = new Sql($"SELECT o.{oId}, p.* FROM {dbOrders} o INNER JOIN {dbPeople} p ON o.{oPersonId} = p.{pId}");

            Should.Throw<InvalidOperationException>(() => DB.Fetch<ReadOnlyMultiPoco, JoinablePerson>(sql).ToList());
        }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForPocoGivenSql_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void FetchWithPaging_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForPocoGivenSql_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task FetchAsyncWithPaging_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForPocoGivenSql_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void SkipAndTake_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForPocoGivenSql_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task SkipAndTakeAsync_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection() => await Task.CompletedTask;

        [Fact]
        public override void QueryMultiple_ForSingleResultsSetWithSinglePoco_ShouldReturnValidPocoCollection()
        {
            AddPeople(1, 0);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;

            var sql = $@"SELECT *
                         FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}
                         WHERE {DB.Provider.EscapeSqlIdentifier(pdName)} LIKE @0 & '%';";

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

        // FIXME: Shouldly.ShouldAssertException : result.Count should be 1 but was 0
        [Fact(Skip = "Limited support for QueryMultiple by provider due to need for multiple statements in a single command.")]
        public override void QueryMultiple_ForSingleResultsSetWithMultiPoco_ShouldReturnValidPocoCollection()
        {
            AddJoinableOrders(1);

            var pd = PocoData.ForType(typeof(JoinablePerson), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(JoinableOrder), DB.DefaultMapper);
            var pdId = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;
            var odPersonId = od.Columns.Values.First(c => c.PropertyInfo.Name == "JoinablePersonId").ColumnName;

            /*
            "SELECT TOP 1 *\r\n                         FROM [JoinableOrders] o\r\n                         INNER JOIN [JoinablePeople] p ON p.[Id] = o.[JoinablePersonId]\r\n                         WHERE p.[FullName] = @0\r\n                         ORDER BY 1 DESC;"

            SELECT TOP 1 *
                         FROM [JoinableOrders] o
                         INNER JOIN [JoinablePeople] p ON p.[Id] = o.[JoinablePersonId]
                         WHERE p.[FullName] = @0
                         ORDER BY 1 DESC;
            */

            var sql = $@"SELECT TOP 1 *
                         FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
                         INNER JOIN {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p ON p.{DB.Provider.EscapeSqlIdentifier(pdId)} = o.{DB.Provider.EscapeSqlIdentifier(odPersonId)}
                         WHERE p.{DB.Provider.EscapeSqlIdentifier(pdName)} = @0
                         ORDER BY 1 DESC;";

            List<JoinableOrder> result;
            using (var multi = DB.QueryMultiple(sql, "Peta0"))
            {
                result = multi.Read<JoinableOrder, JoinablePerson, JoinableOrder>((o, p) =>
                {
                    o.JoinablePerson = p;
                    return o;
                }).ToList();
            }

            result.Count.ShouldBe(1);

            var order = result.First();

            order.PoNumber.ShouldStartWith("PO");
            order.Status.ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToArray());
            order.JoinablePersonId.ShouldNotBe(0);
            order.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            order.CreatedBy.ShouldStartWith("Harry");

            order.JoinablePerson.ShouldNotBeNull();
            order.JoinablePerson.Id.ShouldNotBe(0);
            order.JoinablePerson.Name.ShouldStartWith("Peta");
            order.JoinablePerson.Age.ShouldBe(18);
        }

        // FIXME: System.Data.OleDb.OleDbException : Characters found after end of SQL statement.
        [Fact(Skip = "Limited support for QueryMultiple by provider due to need for multiple statements in a single command.")]
        public override void QueryMultiple_ForMultiResultsSetWithSinglePoco_ShouldReturnValidPocoCollection()
        {
            AddOrders(1);

            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var pdName = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName;
            var odId = od.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;

            /*
            "SELECT * FROM [Orders] o\r\n                         WHERE o.[Id] = @0;\r\n                         SELECT * FROM [People] p\r\n                         WHERE p.[FullName] = @1;"

            SELECT * FROM [Orders] o
                         WHERE o.[Id] = @0;
                         SELECT * FROM [People] p
                         WHERE p.[FullName] = @1;
            */
            var sql = $@"SELECT * FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
                         WHERE o.{DB.Provider.EscapeSqlIdentifier(odId)} = @0;
                         SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p
                         WHERE p.{DB.Provider.EscapeSqlIdentifier(pdName)} = @1";

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

        // FIXME: System.Data.OleDb.OleDbException : Characters found after end of SQL statement.
        [Fact(Skip = "Limited support for QueryMultiple by provider due to need for multiple statements in a single command.")]
        public override void QueryMultiple_ForMultiResultsSetWithMultiPoco_ShouldReturnValidPocoCollection()
        {
            AddJoinableOrders(12);

            var pd = PocoData.ForType(typeof(JoinablePerson), DB.DefaultMapper);
            var od = PocoData.ForType(typeof(JoinableOrder), DB.DefaultMapper);
            var old = PocoData.ForType(typeof(JoinableOrderLine), DB.DefaultMapper);
            var pdId = pd.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var odId = od.Columns.Values.First(c => c.PropertyInfo.Name == "Id").ColumnName;
            var odPersonId = od.Columns.Values.First(c => c.PropertyInfo.Name == "JoinablePersonId").ColumnName;
            var oldOrderId = old.Columns.Values.First(c => c.PropertyInfo.Name == "JoinableOrderId").ColumnName;

            /*
            "SELECT * FROM [JoinableOrders] o\r\n                         INNER JOIN [JoinablePeople] p ON p.[Id] = o.[JoinablePersonId]\r\n                         ORDER BY o.[Id] ASC;\r\n                         SELECT * FROM [JoinableOrderLines] ol\r\n                         ORDER BY ol.[JoinableOrderId] ASC;"

            SELECT * FROM [JoinableOrders] o
                         INNER JOIN [JoinablePeople] p ON p.[Id] = o.[JoinablePersonId]
                         ORDER BY o.[Id] ASC;
                         SELECT * FROM [JoinableOrderLines] ol
                         ORDER BY ol.[JoinableOrderId] ASC;
            */
            var sql = $@"SELECT * FROM {DB.Provider.EscapeTableName(od.TableInfo.TableName)} o
                         INNER JOIN {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} p ON p.{DB.Provider.EscapeSqlIdentifier(pdId)} = o.{DB.Provider.EscapeSqlIdentifier(odPersonId)}
                         ORDER BY o.{DB.Provider.EscapeSqlIdentifier(odId)} ASC;
                         SELECT * FROM {DB.Provider.EscapeTableName(old.TableInfo.TableName)} ol
                         ORDER BY ol.{DB.Provider.EscapeSqlIdentifier(oldOrderId)} ASC;";

            List<JoinableOrder> results;
            using (var multi = DB.QueryMultiple(sql))
            {
                results = multi.Read<JoinableOrder, JoinablePerson, JoinableOrder>((o, p) =>
                {
                    o.JoinablePerson = p;
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
                o.JoinablePersonId.ShouldNotBe(0);
                o.CreatedOn.ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                o.CreatedBy.ShouldStartWith("Harry");

                o.JoinablePerson.ShouldNotBeNull();
                o.JoinablePerson.Id.ShouldNotBe(0);
                o.JoinablePerson.Name.ShouldStartWith("Peta");
                o.JoinablePerson.Age.ShouldBeGreaterThanOrEqualTo(18);

                o.OrderLines.Count.ShouldBe(2);

                var firstOrderLine = o.OrderLines.First();
                firstOrderLine.Quantity.ToString().ShouldBe("1");
                firstOrderLine.SellPrice.ShouldBe(9.99m);

                var secondOrderLine = o.OrderLines.Skip(1).First();
                secondOrderLine.Quantity.ToString().ShouldBe("2");
                secondOrderLine.SellPrice.ShouldBe(19.98m);
            });
        }

        #region Helpers

        protected void AddJoinableOrders(int ordersToAdd)
        {
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray();
            var people = new List<JoinablePerson>(4);

            for (var i = 0; i < 4; i++)
            {
                var p = new JoinablePerson
                {
                    Id = i,
                    Name = "Peta" + i + 1,
                    Age = 18 + i,
                    Dob = new DateTime(1980 - (18 + 1), 1, 1, 1, 1, 1, DateTimeKind.Utc),
                };
                DB.Insert(p);
                people.Add(p);
            }

            for (var i = 0; i < ordersToAdd; i++)
            {
                var order = new JoinableOrder
                {
                    PoNumber = "PO" + i,
                    Status = (OrderStatus)orderStatuses.GetValue(i % orderStatuses.Length),
                    JoinablePersonId = people.Skip(i % 4).Take(1).Single().Id,
                    CreatedOn = new DateTime(1990 - (i % 4), 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "Harry" + i
                };
                DB.Insert(order);

                for (var j = 1; j <= 2; j++)
                {
                    DB.Insert(new JoinableOrderLine
                    {
                        JoinableOrderId = order.Id,
                        Quantity = (short)j,
                        SellPrice = 9.99m * j
                    });
                }
            }
        }

        #endregion
    }
}
