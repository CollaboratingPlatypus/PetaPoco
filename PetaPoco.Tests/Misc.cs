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
			Assert.AreEqual(db._dbType.EscapeSqlIdentifier("column.name"), "[column.name]");
			Assert.AreEqual(db._dbType.EscapeSqlIdentifier("column name"), "[column name]");
		}

		[Test]
		public void EscapeTableName()
		{
			Assert.AreEqual(db._dbType.EscapeTableName("column.name"), "column.name");
			Assert.AreEqual(db._dbType.EscapeTableName("column name"), "[column name]");
		}
	}
}
