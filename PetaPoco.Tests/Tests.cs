using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PetaPoco;

namespace PetaPoco.Tests
{
	[TestFixture("sqlserver")]
	[TestFixture("sqlserverce")]
	[TestFixture("mysql")]
	[TestFixture("postgresql")]
	public class Tests : AssertionHelper
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

			// Should be clean
			Expect(GetRecordCount(), Is.EqualTo(0));
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

			return o;
		}

		void Assert(poco a, poco b)
		{
			Expect(a.id, Is.EqualTo(b.id));
			Expect(a.title, Is.EqualTo(b.title));
			Expect(a.draft, Is.EqualTo(b.draft));
			Expect(a.content, Is.EqualTo(b.content));
			Expect(a.date_created, Is.EqualTo(b.date_created));
			Expect(a.date_edited, Is.EqualTo(b.date_edited));
			Expect(a.state, Is.EqualTo(b.state));
		}

		void Assert(deco a, deco b)
		{
			Expect(a.id, Is.EqualTo(b.id));
			Expect(a.title, Is.EqualTo(b.title));
			Expect(a.draft, Is.EqualTo(b.draft));
			Expect(a.content, Is.EqualTo(b.content));
			Expect(a.date_created, Is.EqualTo(b.date_created));
			Expect(a.state, Is.EqualTo(b.state));
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
					Expect(o.id, Is.Not.EqualTo(0));
				}
			}

			return lFirst;
		}

		[Test]
		public void poco_Crud()
		{
			// Create a random record
			var o = CreatePoco();

			Expect(db.IsNew("id", o), Is.True);

			// Insert it
			db.Insert("petapoco", "id", o);
			Expect(o.id, Is.Not.EqualTo(0));

			Expect(db.IsNew("id", o), Is.False);

			// Retrieve it
			var o2 = db.Single<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Expect(db.IsNew("id", o2), Is.False);

			// Check it
			Assert(o, o2);

			// Update it
			o2.title = "New Title";
			db.Save("petapoco", "id", o2);

			// Retrieve itagain
			var o3 = db.Single<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			// Check it
			Assert(o2, o3);

			// Delete it
			db.Delete("petapoco", "id", o3);

			// Should be gone!
			var o4 = db.SingleOrDefault<poco>("SELECT * FROM petapoco WHERE id=@0", o.id);
			Expect(o4, Is.Null);
		}

		[Test]
		public void deco_Crud()
		{
			// Create a random record
			var o = CreateDeco();
			Expect(db.IsNew(o), Is.True);

			// Insert it
			db.Insert(o);
			Expect(o.id, Is.Not.EqualTo(0));

			Expect(db.IsNew(o), Is.False);
			
			// Retrieve it
			var o2 = db.Single<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Expect(db.IsNew(o2), Is.False);

			// Check it
			Assert(o, o2);

			// Update it
			o2.title = "New Title";
			db.Save(o2);

			// Retrieve itagain
			var o3 = db.Single<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);

			// Check it
			Assert(o2, o3);

			// Delete it
			db.Delete(o3);

			// Should be gone!
			var o4 = db.SingleOrDefault<deco>("SELECT * FROM petapoco WHERE id=@0", o.id);
			Expect(o4, Is.Null);
		}

		[Test]
		public void Fetch()
		{
			// Create some records
			const int count = 5;
			long id = InsertRecords(count);

			// Fetch em
			var r = db.Fetch<poco>("SELECT * from petapoco ORDER BY id");
			Expect(r.Count, Is.EqualTo(count));

			// Check em
			for (int i = 0; i < count; i++)
			{
				Expect(r[i].id, Is.EqualTo(id + i));
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
				Expect(p.id, Is.EqualTo(id + i));
				i++;
			}
			Expect(i, Is.EqualTo(count));
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
				Expect(p.id, Is.EqualTo(id + i + 5));
				i++;
			}

			// Check other stats
			Expect(r.Items.Count, Is.EqualTo(5));
			Expect(r.CurrentPage, Is.EqualTo(2));
			Expect(r.ItemsPerPage, Is.EqualTo(5));
			Expect(r.TotalItems, Is.EqualTo(13));
			Expect(r.TotalPages, Is.EqualTo(3));
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
				Expect(p.id, Is.EqualTo(id + i + 5));
				i++;
			}

			// Check other stats
			Expect(r.Count, Is.EqualTo(5));
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
			Expect(r.Items.Count, Is.EqualTo(5));
			Expect(r.CurrentPage, Is.EqualTo(3));
			Expect(r.ItemsPerPage, Is.EqualTo(5));
			Expect(r.TotalItems, Is.EqualTo(15));
			Expect(r.TotalPages, Is.EqualTo(3));
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
			Expect(GetRecordCount(), Is.EqualTo(5));
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
					Expect(d.title, Is.EqualTo("zap"));
				}
				else
				{
					Expect(d.title, Is.Not.EqualTo("zap"));
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
			Expect(a.content, Is.Not.Null);
			Expect(b.content, Is.Null);
			Expect(c.content, Is.Null);
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
			Expect(a.content, Is.Not.Null);
			Expect(b.content, Is.Null);
			Expect(c.content, Is.Null);
		}

		[Test]
		public void Transaction_complete()
		{
			using (var scope = db.Transaction)
			{
				InsertRecords(10);
				scope.Complete();
			}

			Expect(GetRecordCount(), Is.EqualTo(10));
		}

		[Test]
		public void Transaction_cancelled()
		{
			using (var scope = db.Transaction)
			{
				InsertRecords(10);
			}

			Expect(GetRecordCount(), Is.EqualTo(0));
		}

		[Test]
		public void Transaction_nested_nn()
		{
			using (var scope1 = db.Transaction)
			{
				InsertRecords(10);

				using (var scope2 = db.Transaction)
				{
					InsertRecords(10);
				}
			}

			Expect(GetRecordCount(), Is.EqualTo(0));
		}

		[Test]
		public void Transaction_nested_yn()
		{
			using (var scope1 = db.Transaction)
			{
				InsertRecords(10);

				using (var scope2 = db.Transaction)
				{
					InsertRecords(10);
				}
				scope1.Complete();
			}

			Expect(GetRecordCount(), Is.EqualTo(0));
		}

		[Test]
		public void Transaction_nested_ny()
		{
			using (var scope1 = db.Transaction)
			{
				InsertRecords(10);

				using (var scope2 = db.Transaction)
				{
					InsertRecords(10);
					scope2.Complete();
				}
			}

			Expect(GetRecordCount(), Is.EqualTo(0));
		}

		[Test]
		public void Transaction_nested_yy()
		{
			using (var scope1 = db.Transaction)
			{
				InsertRecords(10);

				using (var scope2 = db.Transaction)
				{
					InsertRecords(10);
					scope2.Complete();
				}

				scope1.Complete();
			}

			Expect(GetRecordCount(), Is.EqualTo(20));
		}

		[Test]
		public void Transaction_nested_yny()
		{
			using (var scope1 = db.Transaction)
			{
				InsertRecords(10);

				using (var scope2 = db.Transaction)
				{
					InsertRecords(10);
					//scope2.Complete();
				}

				using (var scope3 = db.Transaction)
				{
					InsertRecords(10);
					scope3.Complete();
				}

				scope1.Complete();
			}

			Expect(GetRecordCount(), Is.EqualTo(0));
		}

		[Test]
		public void DateTimesAreUtc()
		{
			var id = InsertRecords(1);
			var a2 = db.SingleOrDefault<deco>("WHERE id=@0", id);
			Expect(a2.date_created.Kind, Is.EqualTo(DateTimeKind.Utc));
			Expect(a2.date_edited.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
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
			Expect(b.id, Is.EqualTo(a.id));
			Expect(b.date_edited.HasValue, Is.EqualTo(false));

			// Update it to NULL
			b.date_edited = now;
			db.Update(b);
			var c = db.SingleOrDefault<deco>("WHERE id=@0", a.id);
			Expect(c.id, Is.EqualTo(a.id));
			Expect(c.date_edited.HasValue, Is.EqualTo(true));

			// Update it to not NULL
			c.date_edited = null;
			db.Update(c);
			var d = db.SingleOrDefault<deco>("WHERE id=@0", a.id);
			Expect(d.id, Is.EqualTo(a.id));
			Expect(d.date_edited.HasValue, Is.EqualTo(false));
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
			Expect(items.Count, Is.EqualTo(4));
		}

		[Test]
		public void SingleOrDefault_Empty()
		{
			Expect(db.SingleOrDefault<deco>("WHERE id=@0", 0), Is.Null);
		}

		[Test]
		public void SingleOrDefault_Single()
		{
			var id = InsertRecords(1);
			Expect(db.SingleOrDefault<deco>("WHERE id=@0", id), Is.Not.Null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SingleOrDefault_Multiple()
		{
			var id = InsertRecords(2);
			db.SingleOrDefault<deco>("WHERE id>=@0", id);
		}

		[Test]
		public void FirstOrDefault_Empty()
		{
			Expect(db.FirstOrDefault<deco>("WHERE id=@0", 0), Is.Null);
		}

		[Test]
		public void FirstOrDefault_First()
		{
			var id = InsertRecords(1);
			Expect(db.FirstOrDefault<deco>("WHERE id=@0", id), Is.Not.Null);
		}

		[Test]
		public void FirstOrDefault_Multiple()
		{
			var id = InsertRecords(2);
			Expect(db.FirstOrDefault<deco>("WHERE id>=@0", id), Is.Not.Null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Single_Empty()
		{
			db.Single<deco>("WHERE id=@0", 0);
		}

		[Test]
		public void Single_Single()
		{
			var id = InsertRecords(1);
			Expect(db.Single<deco>("WHERE id=@0", id), Is.Not.Null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Single_Multiple()
		{
			var id = InsertRecords(2);
			db.Single<deco>("WHERE id>=@0", id);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void First_Empty()
		{
			db.First<deco>("WHERE id=@0", 0);
		}

		[Test]
		public void First_First()
		{
			var id = InsertRecords(1);
			Expect(db.First<deco>("WHERE id=@0", id), Is.Not.Null);
		}

		[Test]
		public void First_Multiple()
		{
			var id = InsertRecords(2);
			Expect(db.First<deco>("WHERE id>=@0", id), Is.Not.Null);
		}

		[Test]
		public void SingleOrDefault_PK_Empty()
		{
			Expect(db.SingleOrDefault<deco>(0), Is.Null);
		}

		[Test]
		public void SingleOrDefault_PK_Single()
		{
			var id = InsertRecords(1);
			Expect(db.SingleOrDefault<deco>(id), Is.Not.Null);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Single_PK_Empty()
		{
			db.Single<deco>(0);
		}

		[Test]
		public void Single_PK_Single()
		{
			var id = InsertRecords(1);
			Expect(db.Single<deco>(id), Is.Not.Null);
		}

		[Test]
		public void AutoSelect_SelectPresent()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("SELECT * FROM petapoco WHERE id=@0", id);
			Expect(a, Is.Not.Null);
			Expect(a.id, Is.EqualTo(id));
		}

		[Test]
		public void AutoSelect_SelectMissingFromMissing()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("WHERE id=@0", id);
			Expect(a, Is.Not.Null);
			Expect(a.id, Is.EqualTo(id));
		}

		[Test]
		public void AutoSelect_SelectMissingFromPresent()
		{
			var id = InsertRecords(1);
			var a = db.SingleOrDefault<deco>("FROM petapoco WHERE id=@0", id);
			Expect(a, Is.Not.Null);
			Expect(a.id, Is.EqualTo(id));
		}

		void AssertDynamic(dynamic a, dynamic b)
		{
			Expect(a.id, Is.EqualTo(b.id));
			Expect(a.title, Is.EqualTo(b.title));
			Expect(a.draft, Is.EqualTo(b.draft));
			Expect(a.content, Is.EqualTo(b.content));
			Expect(a.date_created, Is.EqualTo(b.date_created));
			Expect(a.state, Is.EqualTo(b.state));
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

			Expect(db.IsNew("id", o), Is.True);

			// Insert it
			db.Insert("petapoco", "id", o);
			Expect(o.id, Is.Not.EqualTo(0));

			Expect(db.IsNew("id", o), Is.False);

			// Retrieve it
			var o2 = db.Single<dynamic>("SELECT * FROM petapoco WHERE id=@0", o.id);

			Expect(db.IsNew("id", o2), Is.False);

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
			Expect(o4==null, Is.True);
		}
	}

}
