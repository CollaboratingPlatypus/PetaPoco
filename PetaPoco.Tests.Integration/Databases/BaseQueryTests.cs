// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/02/04</date>

using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Providers;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseQueryTests : BaseDatabase
    {
        protected BaseQueryTests(DBTestProvider provider)
            : base(provider)
        {
        }

        [Fact]
        public virtual void Page_ForPocoGivenSqlWithoutOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            AddPeople(15, 5);
            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var sql = $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} LIKE @0";

            var page = DB.Page<Person>(2, 3, sql, "Peta%");
            page.CurrentPage.ShouldBe(2);
            page.TotalPages.ShouldBe(5);
            page.TotalItems.ShouldBe(15);
            page.ItemsPerPage.ShouldBe(3);
            page.Items.Count.ShouldBe(3);
            page.Items.All(p => p.Name.StartsWith("Peta")).ShouldBeTrue();
        }

        [Fact]
        public virtual void Page_ForPocoSqlWithOrderByParameterPageItemAndPerPage_ShouldReturnValidPocoCollection()
        {
            AddPeople(15, 5);
            var pd = PocoData.ForType(typeof(Person), DB.DefaultMapper);
            var sql = $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)} LIKE @0 " +
                      $"ORDER BY {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Name").ColumnName)}";

            var page = DB.Page<Person>(2, 3, sql, "Peta%");
            page.CurrentPage.ShouldBe(2);
            page.TotalPages.ShouldBe(5);
            page.TotalItems.ShouldBe(15);
            page.ItemsPerPage.ShouldBe(3);
            page.Items.Count.ShouldBe(3);
            page.Items.All(p => p.Name.StartsWith("Peta")).ShouldBeTrue();
        }

        [Fact]
        public void Query_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =
                $"SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} " +
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Query<dynamic>(sql, OrderStatus.Pending);
            results.Count().ShouldBe(3);

            var order = (IDictionary<string, object>) results.First();
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName]).ShouldStartWith("PO");
            Convert.ToInt32(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName])
                .ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray());
            order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName].ToString().ShouldNotBe(Guid.Empty.ToString());
            ConvertToDateTime(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedOn").ColumnName])
                .ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedBy").ColumnName]).ShouldStartWith("Harry");
        }

        [Fact]
        public void Query_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =
                new Sql(
                    $"SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} " +
                    $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                    OrderStatus.Pending);

            var results = DB.Query<dynamic>(sql);
            results.Count().ShouldBe(3);

            var order = (IDictionary<string, object>) results.First();
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName]).ShouldStartWith("PO");
            Convert.ToInt32(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName])
                .ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray());
            order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName].ToString().ShouldNotBe(Guid.Empty.ToString());
            ConvertToDateTime(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedOn").ColumnName])
                .ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedBy").ColumnName]).ShouldStartWith("Harry");
        }

        [Fact]
        public void Query_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

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
        public void Query_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);

            var sql =
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @Status AND @NullableProperty IS NULL";

            if (DB.Provider.GetType() == typeof(PostgreSQLDatabaseProvider))
            {
                sql = sql.Replace("@NullableProperty", "CAST(@NullableProperty AS CHAR)");
            }
            else if (DB.Provider.GetType() == typeof(SqlServerCEDatabaseProviders))
            {
                sql = sql.Replace("@NullableProperty", "CAST(@NullableProperty AS NTEXT)");
            }

            var results = DB.Query<Order>(sql, new { Status = OrderStatus.Pending, NullableProperty = (string)null }).ToList();
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
        public void Query_ForPocoGivenSql_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                OrderStatus.Pending);

            var results = DB.Query<Order>(sql).ToList();
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
        public void Query_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName)} " +
                      $"FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}" +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Query<string>(sql, OrderStatus.Pending).ToList();
            results.Count.ShouldBe(3);
            results.ForEach(po => po.ShouldStartWith("PO"));
        }

        [Fact]
        public void Query_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql($"SELECT {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName)} " +
                              $"FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}" +
                              $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                OrderStatus.Pending);

            var results = DB.Query<string>(sql).ToList();
            results.Count.ShouldBe(3);
            results.ForEach(po => po.ShouldStartWith("PO"));
        }

        [Fact]
        public void Fetch_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =
                $"SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} " +
                $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Fetch<dynamic>(sql, OrderStatus.Pending);
            results.Count.ShouldBe(3);

            var order = (IDictionary<string, object>) results.First();
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName]).ShouldStartWith("PO");
            Convert.ToInt32(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName])
                .ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray());
            order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName].ToString().ShouldNotBe(Guid.Empty.ToString());
            ConvertToDateTime(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedOn").ColumnName])
                .ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedBy").ColumnName]).ShouldStartWith("Harry");
        }

        [Fact]
        public void Fetch_ForDynamicTypeGivenSql_ShouldReturnValidDynamicTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =
                new Sql(
                    $"SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} " +
                    $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                    OrderStatus.Pending);

            var results = DB.Fetch<dynamic>(sql);
            results.Count.ShouldBe(3);

            var order = (IDictionary<string, object>) results.First();
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName]).ShouldStartWith("PO");
            Convert.ToInt32(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName])
                .ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray());
            order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName].ToString().ShouldNotBe(Guid.Empty.ToString());
            ConvertToDateTime(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedOn").ColumnName])
                .ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedBy").ColumnName]).ShouldStartWith("Harry");
        }

        [Fact]
        public void Fetch_ForPocoGivenSqlStringAndParameters_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =$"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @Status AND @NullableProperty IS NULL";

            if (DB.Provider.GetType() == typeof(PostgreSQLDatabaseProvider))
            {
                sql = sql.Replace("@NullableProperty", "CAST(@NullableProperty AS CHAR)");
            }
            else if (DB.Provider.GetType() == typeof(SqlServerCEDatabaseProviders))
            {
                sql = sql.Replace("@NullableProperty", "CAST(@NullableProperty AS NTEXT)");
            }

            var results = DB.Fetch<Order>(sql, new { Status = OrderStatus.Pending, NullableProperty = (string)null });
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
        public void Fetch_ForPocoGivenSqlStringAndNamedParameters_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Fetch<Order>(sql, OrderStatus.Pending);
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
        public void Fetch_ForPocoGivenSql_ShouldReturnValidPocoCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql($"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                OrderStatus.Pending);

            var results = DB.Fetch<Order>(sql);
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
        public void Fetch_ForValueTypeGivenSqlStringAndParameters_ShouldReturnValidValueTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = $"SELECT {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName)} " +
                      $"FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}" +
                      $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Fetch<string>(sql, OrderStatus.Pending);
            results.Count.ShouldBe(3);
            results.ForEach(po => po.ShouldStartWith("PO"));
        }

        [Fact]
        public void Fetch_ForValueTypeGivenSql_ShouldReturnValidValueTypeCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql = new Sql($"SELECT {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName)} " +
                              $"FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)}" +
                              $"WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0",
                OrderStatus.Pending);

            var results = DB.Fetch<string>(sql);
            results.Count.ShouldBe(3);
            results.ForEach(po => po.ShouldStartWith("PO"));
        }

        protected void AddPeople(int petasToAdd, int sallysToAdd)
        {
            var c = petasToAdd > sallysToAdd ? petasToAdd : sallysToAdd;
            for (var i = 0; i < c; i++)
            {
                if (petasToAdd > i)
                {
                    DB.Insert(new Person
                    {
                        Id = Guid.NewGuid(),
                        Name = "Peta" + i,
                        Age = 18 + i,
                        Dob = new DateTime(1980 - (18 + 1), 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    });
                }
                if (sallysToAdd > i)
                {
                    DB.Insert(new Person
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sally" + i,
                        Age = 18 + i,
                        Dob = new DateTime(1980 - (18 + 1), 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    });
                }
            }
        }

        protected void AddOrders(int ordersToAdd)
        {
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray();
            var people = new List<Person>(4);

            for (var i = 0; i < 4; i++)
            {
                var p = new Person
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
                var order = new Order
                {
                    PoNumber = "PO" + i,
                    Status = (OrderStatus) orderStatuses.GetValue(i % orderStatuses.Length),
                    PersonId = people.Skip(i % 4).Take(1).Single().Id,
                    CreatedOn = new DateTime(1990 - (i % 4), 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "Harry" + i
                };
                DB.Insert(order);

                for (var j = 1; j <= 2; j++)
                {
                    DB.Insert(new OrderLine
                    {
                        OrderId = order.Id,
                        Quantity = (short) j,
                        SellPrice = 9.99m * j
                    });
                }
            }
        }

        protected static DateTime ConvertToDateTime(object value)
        {
            return value as DateTime? ?? new DateTime((long) value, DateTimeKind.Utc);
        }
    }
}