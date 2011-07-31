using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;

namespace PetaPoco.Tests
{
	[TestFixture]
	class Misc
	{
		Database db = new Database("sqlserver");

		[Test]
		public void EscapeColumnName()
		{
			Assert.AreEqual(db.EscapeSqlIdentifier("column.name"), "[column.name]");
			Assert.AreEqual(db.EscapeSqlIdentifier("column name"), "[column name]");
		}

		[Test]
		public void EscapeTableName()
		{
			Assert.AreEqual(db.EscapeTableName("column.name"), "column.name");
			Assert.AreEqual(db.EscapeTableName("column name"), "[column name]");
		}
	}
}
