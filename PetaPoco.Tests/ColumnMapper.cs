using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PetaPoco.Tests
{
	public class Poco2
	{
		public string prop1 { get; set; }
		public string prop2 { get; set; }
		public string prop3 { get; set; }
		public string prop4 { get; set; }
	}

	public class MyColumnMapper : PetaPoco.IMapper
	{
		public void GetTableInfo(Type t, ref string tableName, ref string primaryKey, ref string sequenceName)
		{
			if (t == typeof(Poco2))
			{
				tableName = "petapoco";
				primaryKey = "id";
			}
		}
		public bool MapPropertyToColumn(System.Reflection.PropertyInfo pi, ref string columnName, ref bool resultColumn)
		{
			if (pi.DeclaringType == typeof(Poco2))
			{
				switch (pi.Name)
				{
					case "prop1":
						// Leave this property as is
						return true;

					case "prop2":
						// Rename this column
						columnName = "remapped2";
						return true;

					case "prop3":
						// Mark this as a result column
						resultColumn = true;
						return true;

					case "prop4":
						// Ignore this property
						return false;
				}
			}

			// Do default property mapping
			return true;
		}


		public Func<object, object> GetFromDbConverter(System.Reflection.PropertyInfo pi, Type SourceType)
		{
			return null;
		}

		public Func<object, object> GetToDbConverter(Type SourceType)
		{
			return null;
		}
	}

	[TestFixture]
	public class ColumnMapper : AssertionHelper
	{


		[Test]
		public void NoColumnMapper()
		{

			PetaPoco.Database.Mapper = new MyColumnMapper();
			var pd=PetaPoco.Database.PocoData.ForType(typeof(Poco2));

			Expect(pd.Columns.Count, Is.EqualTo(3));
			Expect(pd.Columns["prop1"].PropertyInfo.Name, Is.EqualTo("prop1"));
			Expect(pd.Columns["remapped2"].ColumnName, Is.EqualTo("remapped2"));
			Expect(pd.Columns["prop3"].ColumnName, Is.EqualTo("prop3"));
			Expect(pd.QueryColumns, Is.EqualTo("prop1, remapped2"));
			Expect(pd.PrimaryKey, Is.EqualTo("id"));
			Expect(pd.TableName, Is.EqualTo("petapoco"));
		}
	}
}
