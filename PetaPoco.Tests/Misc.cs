using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PetaPoco.Tests
{
	[TestFixture]
	class Misc : AssertionHelper
	{
		Database db = new Database("sqlserver");

		[Test]
		public void EscapeColumnName()
		{
			Expect(db.EscapeSqlIdentifier("column.name"), Is.EqualTo("[column.name]"));
			Expect(db.EscapeSqlIdentifier("column name"), Is.EqualTo("[column name]"));
		}

		[Test]
		public void EscapeTableName()
		{
			Expect(db.EscapeTableName("column.name"), Is.EqualTo("column.name"));
			Expect(db.EscapeTableName("column name"), Is.EqualTo("[column name]"));
		}
	}
}
