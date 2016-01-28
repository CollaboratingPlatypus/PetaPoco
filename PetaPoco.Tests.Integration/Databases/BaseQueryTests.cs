// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/28</date>

using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco.Core;
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
        public void Query_ForDynamicTypeGivenSqlStringAndParameters_ShouldReturnValidCollection()
        {
            AddOrders(12);
            var pd = PocoData.ForType(typeof(Order), DB.DefaultMapper);
            var sql =
                $"SELECT * FROM {DB.Provider.EscapeTableName(pd.TableInfo.TableName)} WHERE {DB.Provider.EscapeSqlIdentifier(pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName)} = @0";

            var results = DB.Query<dynamic>(sql, OrderStatus.Pending);
            results.Count().ShouldBe(3);

            var order = (IDictionary<string, object>) results.First();
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PoNumber").ColumnName]).ShouldStartWith("PO");
            Convert.ToInt32(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "Status").ColumnName]).ShouldBeOneOf(Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray());
            order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "PersonId").ColumnName].ToString().ShouldNotBe(Guid.Empty.ToString());
            ConvertToDateTime(order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedOn").ColumnName]).ShouldBeLessThanOrEqualTo(new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            ((string) order[pd.Columns.Values.First(c => c.PropertyInfo.Name == "CreatedBy").ColumnName]).ShouldStartWith("Harry");
        }

        private void AddOrders(int ordersToAdd)
        {
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().ToArray();
            var people = new List<Person>(4);

            for (var i = 0; i < 4; i++)
            {
                var p = new Person
                {
                    Id = Guid.NewGuid(),
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

                for (var j = 0; j < 2; j++)
                {
                    DB.Insert(new OrderLine
                    {
                        OrderId = order.Id,
                        Quantity = (short) j,
                        SellPrice = 9.99m * j,
                        Total = 9.99m * j * j
                    });
                }
            }
        }

        private static DateTime ConvertToDateTime(object value)
        {
            return value as DateTime? ?? new DateTime((long)value, DateTimeKind.Utc);
        }
    }
}