using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using PetaPoco.Tests.Integration.Models.MSAccess;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MsAccessQueryTests : BaseQueryTests
    {
        public MsAccessQueryTests()
            : base(new MsAccessDBTestProvider())
        {
        }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void PageCount_GivenSqlWithGroupBy_ShouldReturnEntireQueryCount() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override void Page_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() { }

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

        [Fact(Skip = "Paging not supported by provider.")]
        public override async Task PageAsync_ForPocoGivenSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection() => await Task.CompletedTask;

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

        #region Helpers

        protected void AddJoinableOrders(int ordersToAdd)
        {
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray();
            var people = new List<JoinablePerson>(4);

            for (var i = 0; i < 4; i++)
            {
                var p = new JoinablePerson
                {
                    Id = Guid.NewGuid(),
                    Name = "Peta" + i,
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
            }
        }

        #endregion
    }
}
