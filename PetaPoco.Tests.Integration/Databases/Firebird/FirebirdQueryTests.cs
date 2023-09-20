using System.Linq;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdQueryTests : BaseQueryTests
    {
        public FirebirdQueryTests()
            : base(new FirebirdDBTestProvider())
        {
        }

		[Fact]
		public override void Query_ForMultiPocoWithWildcard_ShouldReturnValidPocoCollectionAfterColumnAdded()
		{
			AddOrders(1);
			var pdOrder = PocoData.ForType(typeof(Order), DB.DefaultMapper);
			var pdPerson = PocoData.ForType(typeof(Person), DB.DefaultMapper);

			var orderTable = DB.Provider.EscapeTableName(pdOrder.TableInfo.TableName);
			var personTable = DB.Provider.EscapeTableName(pdPerson.TableInfo.TableName);

			var oPersonId = DB.Provider.EscapeSqlIdentifier(pdOrder.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(Order.PersonId)).ColumnName);
			var pId = DB.Provider.EscapeSqlIdentifier(pdPerson.Columns.Values.Single(c => c.PropertyInfo.Name == nameof(Person.Id)).ColumnName);
			var randColumn = DB.Provider.EscapeSqlIdentifier("SomeRandomColumn");

			var testQuery = $"SELECT * FROM {orderTable} o JOIN {personTable} p ON o.{oPersonId} = p.{pId}";

			var results = DB.Query<Order, Person>(testQuery).ToList();
			results.ShouldNotBeEmpty();

			var execStmt = $"ALTER TABLE {orderTable} ADD {randColumn} INT";
			DB.Execute(execStmt);

			results = DB.Query<Order, Person>(testQuery).ToList();
			results.ShouldNotBeEmpty();
		}
	}
}
