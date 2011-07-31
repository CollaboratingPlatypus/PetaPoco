using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using PetaPoco;

namespace PetaPoco.Tests
{
	[TestFixture("sqlserver")]
	[TestFixture("sqlserverce")]
	[TestFixture("mysql")]
	[TestFixture("postgresql")]
	public class Tests
	{
		public Tests(string connectionStringName)
		{
			_connectionStringName = connectionStringName;
		}

		string _connectionStringName;
		Random r = new Random();
		Database db;

		[TestFixtureSetUp]
		public void CreateDB()
		{
			db = new Database(_connectionStringName);
			db.OpenSharedConnection();		// <-- Wow, this is crucial to getting SqlCE to perform.
			db.Execute(Utils.LoadTextResource(string.Format("PetaPoco.Tests.{0}_init.sql", _connectionStringName)));
		}

		[TestFixtureTearDown]
		public void DeleteDB()
		{
			db.Execute(Utils.LoadTextResource(string.Format("PetaPoco.Tests.{0}_done.sql", _connectionStringName)));
		}

		long GetRecordCount()
		{
			return db.ExecuteScalar<long>("SELECT COUNT(*) FROM petapoco");
		}

		[TearDown]
		public void Teardown()
		{
			// Delete everything
			db.Delete<deco>("");
			db.Delete<petapoco2>("");

			// Should be clean
			Assert.AreEqual(GetRecordCount(), 0);
		}

		poco CreatePoco()
		{
			// Need a rounded date as DB can't store millis
			var now = DateTime.UtcNow;
			now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			// Setup a record
			var o = new poco();
			o.title = string.Format("insert {0}", r.Next());
			o.draft = true;
			o.content = string.Format("insert {0}", r.Next());
			o.date_created = now;
			o.date_edited = now;
			o.state = State.Yes;
			o.col_w_space = 23;
			o.nullreal = 24;

			return o;
		}

		deco CreateDeco()
		{
			// Need a rounded date as DB can't store millis
			var now = DateTime.UtcNow;
			now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			// Setup a record
			var o = new deco();
			o.title = string.Format("insert {0}", r.Next());
			o.draft = true;
			o.content = string.Format("insert {0}", r.Next());
			o.date_created = now;
			o.date_edited = now;
			o.state = State.Maybe;
			o.col_w_space = 23;
			o.nullreal = 24;

			return o;
		}

		void AssertPocos(poco a, poco b)
		{
			Assert.AreEqual(a.id, b.id);
			Assert.AreEqual(a.title, b.title);
			Assert.AreEqual(a.draft, b.draft);
			Assert.AreEqual(a.content, b.content);
			Assert.AreEqual(a.date_created, b.date_created);
			Assert.AreEqual(a.date_edited, b.date_edited);
			Assert.AreEqual(a.state, b.state);
			Assert.AreEqual(a.col_w_space, b.col_w_space);
			Assert.AreEqual(a.nullreal, b.nullreal);
		}

		void AssertPocos(deco a, deco b)
		{
			Assert.AreEqual(a.id, b.id);
			Assert.AreEqual(a.title, b.title);
			Assert.AreEqual(a.draft, b.draft);
			Assert.AreEqual(a.content, b.content);
			Assert.AreEqual(a.date_created, b.date_created);
			Assert.AreEqual(a.state, b.state);
			Assert.AreEqual(a.col_w_space, b.col_w_space);
			Assert.AreEqual(a.nullreal, b.nullreal);
		}

		// Insert some records, return the id of the first
		long InsertRecords(int count)
		{
			long lFirst = 0;
			for (int i = 0; i < count; i++)
			{
				var o=CreatePoco();
				db.Insert("petapoco", "id", o);

				var lc = db.LastCommand;

				if (i == 0)
				{
					lFirst = o.id;
					Assert.AreNotEqual(o.id, 0);
				}
			}

			return lFirst;
		}

		[Test]
		public void poco_Crud()
		{
			// Create a random record
			var o = CreatePoco();

			Assert.IsTrue(db.IsNew("id", o));

			// Insert it
			db.Insert("petapoco", "id", o);
			Assert.AreNotEqual(o.id, 0);

			Assert.IsFalse(db.IsNew("id", o));

			// Retrieve it
			var o2 = db.Single<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Assert.IsFalse(db.IsNew("id", o2));

			// Check it
			AssertPocos(o, o2);

			// Update it
			o2.title = "New Title";
			db.Save("petapoco", "id", o2);

			// Retrieve itagain
			var o3 = db.Single<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			// Check it
			AssertPocos(o2, o3);

			// Delete it
			db.Delete("petapoco", "id", o3);

			// Should be gone!
			var o4 = db.SingleOrDefault<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);
			Assert.IsNull(o4);
		}

		[Test]
		public void deco_Crud()
		{
			// Create a random record
			var o = CreateDeco();
			Assert.IsTrue(db.IsNew(o));

			// Insert it
			db.Insert(o);
			Assert.AreNotEqual(o.id, 0);

			Assert.IsFalse(db.IsNew(o));
			
			// Retrieve it
			var o2 = db.Single<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Assert.IsFalse(db.IsNew(o2));

			// Check it
			AssertPocos(o, o2);

			// Update it
			o2.title = "New Title";
			db.Save(o2);

			// Retrieve itagain
			var o3 = db.Single<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			// Check it
			AssertPocos(o2, o3);

			// Delete it
			db.Delete(o3);

			// Should be gone!
			var o4 = db.SingleOrDefault<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);
			Assert.IsNull(o4);
		}

		[Test]
		public void Fetch()
		{
			// Create some records
			const int count = 5;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Fetch<poco>("SELECT * from petapoco ORDER BY id");
			Assert.AreEqual(r.Count, count);

			// Check em
			for (int i = 0; i < count; i++)
			{
				Assert.AreEqual(r[i].id, id + i);
			}

		}

		[Test]
		public void Query()
		{
			// Create some records
			const int count = 5;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Query<poco>("SELECT * from petapoco ORDER BY id");

			// Check em
			int i = 0;
			foreach (var p in r)
			{
				Assert.AreEqual(p.id, id + i);
				i++;
			}
			Assert.AreEqual(i, count);
		}

		[Test]
		public void Page()
		{
			// In this test we're checking that the page count is correct when there are
			// not-exactly pagesize*N records (ie: a partial page at the end)

			// Create some records
			const int count = 13;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Page<poco>(2, 5, "SELECT * from petapoco ORDER BY id");

			// Check em
			int i = 0;
			foreach (var p in r.Items)
			{
				Assert.AreEqual(p.id, id + i + 5);
				i++;
			}

			// Check other stats
			Assert.AreEqual(r.Items.Count, 5);
			Assert.AreEqual(r.CurrentPage, 2);
			Assert.AreEqual(r.ItemsPerPage, 5);
			Assert.AreEqual(r.TotalItems, 13);
			Assert.AreEqual(r.TotalPages, 3);
		}

		[Test]
		public void Page_NoOrderBy()
		{
			// Unordered paging not supported by Compact Edition
			if (_connectionStringName == "sqlserverce")
				return;
			// In this test we're checking that the page count is correct when there are
			// not-exactly pagesize*N records (ie: a partial page at the end)

			// Create some records
			const int count = 13;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Page<poco>(2, 5, "SELECT * from petapoco");

			// Check em
			int i = 0;
			foreach (var p in r.Items)
			{
				Assert.AreEqual(p.id, id + i + 5);
				i++;
			}

			// Check other stats
			Assert.AreEqual(r.Items.Count, 5);
			Assert.AreEqual(r.CurrentPage, 2);
			Assert.AreEqual(r.ItemsPerPage, 5);
			Assert.AreEqual(r.TotalItems, 13);
			Assert.AreEqual(r.TotalPages, 3);
		}

		[Test]
		public void Page_Distinct()
		{
			// Unordered paging not supported by Compact Edition
			if (_connectionStringName == "sqlserverce")
				return;
			// In this test we're checking that the page count is correct when there are
			// not-exactly pagesize*N records (ie: a partial page at the end)

			// Create some records
			const int count = 13;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Page<poco>(2, 5, "SELECT DISTINCT id from petapoco ORDER BY id");

			// Check em
			int i = 0;
			foreach (var p in r.Items)
			{
				Assert.AreEqual(p.id, id + i + 5);
				i++;
			}

			// Check other stats
			Assert.AreEqual(r.Items.Count, 5);
			Assert.AreEqual(r.CurrentPage, 2);
			Assert.AreEqual(r.ItemsPerPage, 5);
			Assert.AreEqual(r.TotalItems, 13);
			Assert.AreEqual(r.TotalPages, 3);
		}

		[Test]
		public void FetchPage()
		{
			// Create some records
			const int count = 13;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Fetch<poco>(2, 5, "SELECT * from petapoco ORDER BY id");

			// Check em
			int i = 0;
			foreach (var p in r)
			{
				Assert.AreEqual(p.id, id + i + 5);
				i++;
			}

			// Check other stats
			Assert.AreEqual(r.Count, 5);
		}

		[Test]
		public void Page_boundary()
		{
			// In this test we're checking that the page count is correct when there are
			// exactly pagesize*N records.

			// Create some records
			const int count = 15;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Page<poco>(3, 5, "SELECT * from petapoco ORDER BY id");

			// Check other stats
			Assert.AreEqual(r.Items.Count, 5);
			Assert.AreEqual(r.CurrentPage, 3);
			Assert.AreEqual(r.ItemsPerPage, 5);
			Assert.AreEqual(r.TotalItems, 15);
			Assert.AreEqual(r.TotalPages, 3);
		}

		[Test]
		public void deco_Delete()
		{
			// Create some records
			const int count = 15;
			long id = InsertRecords(count);

			// Delete some
			db.Delete<deco>("WHERE id>=@0", id + 5);

			// Check they match
			Assert.AreEqual(GetRecordCount(), 5);
		}

		[Test]
		public void deco_Update()
		{
			// Create some records
			const int count = 15;
			long id = InsertRecords(count);

			// Update some
			db.Update<deco>("SET title=@0 WHERE id>=@1", "zap", id + 5);

			// Check some updated
			foreach (var d in db.Query<deco>("ORDER BY Id"))
			{
				if (d.id >= id + 5)
				{
					Assert.AreEqual(d.title, "zap");
				}
				else
				{
					Assert.AreNotEqual(d.title, "zap");
				}
			}
		}

		[Test]
		public void deco_ExplicitAttribute()
		{
			// Create a records
			long id = InsertRecords(1);

			// Retrieve it in two different ways
			var a = db.SingleOrDefault<deco>("WHERE id=@0", id);
			var b = db.SingleOrDefault<deco_explicit>("WHERE id=@0", id);
			var c = db.SingleOrDefault<deco_explicit>("SELECT * FROM petapoco WHERE id=@0", id);

			// b record should have ignored the content
			Assert.IsNotNull(a.content);
			Assert.IsNull(b.content);
			Assert.IsNull(c.content);
		}


		[Test]
		public void deco_IgnoreAttribute()
		{
			// Create a records
			long id = InsertRecords(1);

			// Retrieve it in two different ways
			var a = db.SingleOrDefault<deco>("WHERE id=@0", id);
			var b = db.SingleOrDefault<deco_non_explicit>("WHERE id=@0", id);
			var c = db.SingleOrDefault<deco_non_explicit>("SELECT * FROM petapoco WHERE id=@0", id);

			// b record should have ignored the content
			Assert.IsNotNull(a.content);
			Assert.IsNull(b.content);
			Assert.IsNull(c.content);
		}

		[Test]
		public void Transaction_complete()
		{
			using (var scope = db.GetTransaction())
			{
				InsertRecords(10);
				scope.Complete();
			}

			Assert.AreEqual(GetRecordCount(), 10);
		}

		[Test]
		public void Transaction_cancelled()
		{
			using (var scope = db.GetTransaction())
			{
				InsertRecords(10);
			}

			Assert.AreEqual(GetRecordCount(), 0);
		}

		[Test]
		public void Transaction_nested_nn()
		{
			using (var scope1 = db.GetTransaction())
			{
				InsertRecords(10);

				using (var scope2 = db.GetTransaction())
				{
					InsertRecords(10);
				}
			}

			Assert.AreEqual(GetRecordCount(), 0);
		}

		[Test]
		public void Transaction_nested_yn()
		{
			using (var scope1 = db.GetTransaction())
			{
				InsertRecords(10);

				using (var scope2 = db.GetTransaction())
				{
					InsertRecords(10);
				}
				scope1.Complete();
			}

			Assert.AreEqual(GetRecordCount(), 0);
		}

		[Test]
		public void Transaction_nested_ny()
		{
			using (var scope1 = db.GetTransaction())
			{
				InsertRecords(10);

				using (var scope2 = db.GetTransaction())
				{
					InsertRecords(10);
					scope2.Complete();
				}
			}

			Assert.AreEqual(GetRecordCount(), 0);
		}

		[Test]
		public void Transaction_nested_yy()
		{
			using (var scope1 = db.GetTransaction())
			{
				InsertRecords(10);

				using (var scope2 = db.GetTransaction())
				{
					InsertRecords(10);
					scope2.Complete();
				}

				scope1.Complete();
			}

			Assert.AreEqual(GetRecordCount(), 20);
		}

		[Test]
		public void Transaction_nested_yny()
		{
			using (var scope1 = db.GetTransaction())
			{
				InsertRecords(10);

				using (var scope2 = db.GetTransaction())
				{
					InsertRecords(10);
					//scope2.Complete();
				}

				using (var scope3 = db.GetTransaction())
				{
					InsertRecords(10);
					scope3.Complete();
				}

				scope1.Complete();
			}

			Assert.AreEqual(GetRecordCount(), 0);
		}

		[Test]
		public void DateTimesAreUtc()
		{
			var id = InsertRecords(1);
			var a2 = db.SingleOrDefault<deco>("WHERE id=@0", id);
			Assert.AreEqual(a2.date_created.Kind, DateTimeKind.Utc);
			Assert.AreEqual(a2.date_edited.Value.Kind, DateTimeKind.Utc);
		}

		[Test]
		public void DateTimeNullable()
		{
			// Need a rounded date as DB can't store millis
			var now = DateTime.UtcNow;
			now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			// Setup a record
			var a = new deco();
			a.title = string.Format("insert {0}", r.Next());
			a.draft = true;
			a.content = string.Format("insert {0}", r.Next());
			a.date_created = now;
			a.date_edited = null;

			db.Insert(a);

			// Retrieve it
			var b = db.SingleOrDefault<deco>("WHERE id=@0", a.id);
			Assert.AreEqual(b.id, a.id);
			Assert.AreEqual(b.date_edited.HasValue, false);

			// Update it to NULL
			b.date_edited = now;
			db.Update(b);
			var c = db.SingleOrDefault<deco>("WHERE id=@0", a.id);
			Assert.AreEqual(c.id, a.id);
			Assert.AreEqual(c.date_edited.HasValue, true);

			// Update it to not NULL
			c.date_edited = null;
			db.Update(c);
			var d = db.SingleOrDefault<deco>("WHERE id=@0", a.id);
			Assert.AreEqual(d.id, a.id);
			Assert.AreEqual(d.date_edited.HasValue, false);
		}

		[Test]
		public void NamedArgs()
		{
			long first=InsertRecords(10);

			var items=db.Fetch<deco>("WHERE id >= @min_id AND id <= @max_id", 
						new 
						{ 
							min_id = first + 3, 
							max_id = first + 6 
						}
					);
			Assert.AreEqual(items.Count, 4);
		}

		[Test]
		public void SingleOrDefault_Empty()
		{
			Assert.IsNull(db.SingleOrDefault<deco>("WHERE id=@0", 0));
		}

		[Test]
		public void SingleOrDefault_Single()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.SingleOrDefault<deco>("WHERE id=@0", id));
		}

		[Test]
		public void SingleOrDefault_Multiple()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{

				var id = InsertRecords(2);
				db.SingleOrDefault<deco>("WHERE id>=@0", id);
			});
		}

		[Test]
		public void FirstOrDefault_Empty()
		{
			Assert.IsNull(db.FirstOrDefault<deco>("WHERE id=@0", 0));
		}

		[Test]
		public void FirstOrDefault_First()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.FirstOrDefault<deco>("WHERE id=@0", id));
		}

		[Test]
		public void FirstOrDefault_Multiple()
		{
			var id = InsertRecords(2);
			Assert.IsNotNull(db.FirstOrDefault<deco>("WHERE id>=@0", id));
		}

		[Test]
		public void Single_Empty()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				db.Single<deco>("WHERE id=@0", 0);
			});
		}

		[Test]
		public void Single_Single()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.Single<deco>("WHERE id=@0", id));
		}

		[Test]
		public void Single_Multiple()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var id = InsertRecords(2);
				db.Single<deco>("WHERE id>=@0", id);
			});
		}

		[Test]
		public void First_Empty()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				db.First<deco>("WHERE id=@0", 0);
			});
		}

		[Test]
		public void First_First()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.First<deco>("WHERE id=@0", id));
		}

		[Test]
		public void First_Multiple()
		{
			var id = InsertRecords(2);
			Assert.IsNotNull(db.First<deco>("WHERE id>=@0", id));
		}

		[Test]
		public void SingleOrDefault_PK_Empty()
		{
			Assert.IsNull(db.SingleOrDefault<deco>(0));
		}

		[Test]
		public void SingleOrDefault_PK_Single()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.SingleOrDefault<deco>(id));
		}

		[Test]
		public void Single_PK_Empty()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				db.Single<deco>(0);
			});
		}

		[Test]
		public void Single_PK_Single()
		{
			var id = InsertRecords(1);
			Assert.IsNotNull(db.Single<deco>(id));
		}

		[Test]
		public void AutoSelect_SelectPresent()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("SELECT * FROM petapoco WHERE id=@0", id);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.id, id);
		}

		[Test]
		public void AutoSelect_SelectMissingFromMissing()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("WHERE id=@0", id);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.id, id);
		}

		[Test]
		public void AutoSelect_SelectMissingFromPresent()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("FROM petapoco WHERE id=@0", id);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.id, id);
		}

		void AssertDynamic(dynamic a, dynamic b)
		{
			Assert.AreEqual(a.id, b.id);
			Assert.AreEqual(a.title, b.title);
			Assert.AreEqual(a.draft, b.draft);
			Assert.AreEqual(a.content, b.content);
			Assert.AreEqual(a.date_created, b.date_created);
			Assert.AreEqual(a.state, b.state);
		}

		dynamic CreateExpando()
		{
			// Need a rounded date as DB can't store millis
			var now = DateTime.UtcNow;
			now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

			// Setup a record
			dynamic o = new System.Dynamic.ExpandoObject();
			o.title = string.Format("insert {0}", r.Next());
			o.draft = true;
			o.content = string.Format("insert {0}", r.Next());
			o.date_created = now;
			o.date_edited = now;
			o.state = (int)State.Maybe;

			return o;
		}
		[Test]
		public void Dynamic_Query()
		{
			// Create a random record
			var o = CreateExpando();

			Assert.IsTrue(db.IsNew("id", o));

			// Insert it
			db.Insert("petapoco", "id", o);
			Assert.AreNotEqual(o.id, 0);

			Assert.IsFalse(db.IsNew("id", o));

			// Retrieve it
			var o2 = db.Single<dynamic>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Assert.IsFalse(db.IsNew("id", o2));

			// Check it
			AssertDynamic(o, o2);

			// Update it
			o2.title = "New Title";
			db.Save("petapoco", "id", o2);

			// Retrieve itagain
			var o3 = db.Single<dynamic>("SELECT * FROM petapoco WHERE id=@0", o.id);

			// Check it
			AssertDynamic(o2, o3);

			// Delete it
			db.Delete("petapoco", "id", o3);

			// Should be gone!
			var o4 = db.SingleOrDefault<dynamic>("SELECT * FROM petapoco WHERE id=@0", o.id);
			Assert.IsNull(o4);
		}
	
		[Test]
		public void Manual_PrimaryKey()
		{
			var o=new petapoco2();
			o.email="blah@blah.com";
			o.name="Mr Blah";
			db.Insert(o);

			var o2 = db.SingleOrDefault<petapoco2>("WHERE email=@0", "blah@blah.com");
			Assert.AreEqual(o2.name, "Mr Blah");
		}

		[Test]
		public void SingleValueRequest()
		{
			var id = InsertRecords(1);
			var id2 = db.SingleOrDefault<long>("SELECT id from petapoco WHERE id=@0", id);
			Assert.AreEqual(id, id2);
		}
	}

}
