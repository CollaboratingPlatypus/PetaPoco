using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using PetaPoco;

namespace PetaPoco.Tests
{
	public class MultiPocoTests
	{
		[TableName("posts")]
		[PrimaryKey("id")]
		public class post
		{
			public long id { get; set; }
			public string title { get; set; }
			public long author { get; set; }

			[ResultColumn] public author author_obj { get; set; }
		}

		[TableName("authors")]
		[PrimaryKey("id")]
		public class author
		{
			public long id { get; set; }
			public string name { get; set; }

			[ResultColumn]
			public List<post> posts { get; set; }
		}



		public MultiPocoTests()
		{
			_connectionStringName = "mysql";
		}

		string _connectionStringName;
		Database db;

		[TestFixtureSetUp]
		public void CreateDB()
		{
			db = new Database(_connectionStringName);
			db.Execute(@"

DROP TABLE IF EXISTS posts;
DROP TABLE IF EXISTS authors;

CREATE TABLE posts (
	id				bigint AUTO_INCREMENT NOT NULL,
	title			varchar(127) NOT NULL,
	author			bigint NOT NULL,
	PRIMARY KEY (id)
) ENGINE=INNODB;

CREATE TABLE authors (
	id				bigint AUTO_INCREMENT NOT NULL,
	name			varchar(127) NOT NULL,
	PRIMARY KEY (id)
) ENGINE=INNODB;

			");


			var a1 = new author();
			a1.name = "Bill";
			db.Insert(a1);

			var a2 = new author();
			a2.name = "Ted";
			db.Insert(a2);

			var p = new post();
			p.title = "post1";
			p.author = a1.id;
			db.Insert(p);

			p = new post();
			p.title = "post2";
			p.author = a1.id;
			db.Insert(p);

			p = new post();
			p.title = "post3";
			p.author = a2.id;
			db.Insert(p);

		}

		[TestFixtureTearDown]
		public void DeleteDB()
		{
			db.Execute(@"
DROP TABLE IF EXISTS posts;
DROP TABLE IF EXISTS authors;
			");
		}

		[Test]
		public void Basic()
		{
			var posts = db.Fetch<post, author>("SELECT * FROM posts LEFT JOIN authors ON posts.author = authors.id ORDER BY posts.id");
			Assert.AreEqual(posts.Count, 3);

			Assert.AreEqual(posts[0].id, 1);
			Assert.AreEqual(posts[0].title, "post1");
			Assert.AreEqual(posts[0].author, 1);
			Assert.AreEqual(posts[0].author_obj.name, "Bill");
			Assert.AreEqual(posts[1].id, 2);
			Assert.AreEqual(posts[1].title, "post2");
			Assert.AreEqual(posts[1].author, 1);
			Assert.AreEqual(posts[1].author_obj.name, "Bill");
			Assert.AreEqual(posts[2].id, 3);
			Assert.AreEqual(posts[2].title, "post3");
			Assert.AreEqual(posts[2].author, 2);
			Assert.AreEqual(posts[2].author_obj.name, "Ted");
		}

		[Test]
		public void CustomRelator()
		{
			var posts = db.Fetch<post, author, post>(
				(p,a)=>
					{
						p.author_obj = a;
						return p;
					},
				"SELECT * FROM posts LEFT JOIN authors ON posts.author = authors.id ORDER BY posts.id");

			Assert.AreEqual(posts.Count, 3);
			Assert.AreNotSame(posts[0].author_obj, posts[1].author_obj);
			Assert.AreEqual(posts[0].id, 1);
			Assert.AreEqual(posts[0].title, "post1");
			Assert.AreEqual(posts[0].author, 1);
			Assert.AreEqual(posts[0].author_obj.name, "Bill");
			Assert.AreEqual(posts[1].id, 2);
			Assert.AreEqual(posts[1].title, "post2");
			Assert.AreEqual(posts[1].author, 1);
			Assert.AreEqual(posts[1].author_obj.name, "Bill");
			Assert.AreEqual(posts[2].id, 3);
			Assert.AreEqual(posts[2].title, "post3");
			Assert.AreEqual(posts[2].author, 2);
			Assert.AreEqual(posts[2].author_obj.name, "Ted");
		}

		// Relator callback to do many to one relationship mapping
		class PostAuthorRelator
		{
			// A dictionary of known authors
			Dictionary<long, author> authors = new Dictionary<long, author>();

			public post MapIt(post p, author a)
			{
				// Get existing author object, or if not found store this one
				author aExisting;
				if (authors.TryGetValue(a.id, out aExisting))
					a = aExisting;
				else
					authors.Add(a.id, a);

				// Wire up objects
				p.author_obj = a;
				return p;
			}
		}

		[Test]
		public void ManyToOne()
		{
			// This test uses a custom relator callback to connect posts to existing author instances
			// Note that for each row, an author object is still created - it's just that the duplicates
			// are discarded

			var posts = db.Fetch<post, author, post>(new PostAuthorRelator().MapIt,
				"SELECT * FROM posts LEFT JOIN authors ON posts.author = authors.id ORDER BY posts.id"
				);


			Assert.AreEqual(posts.Count, 3);
			Assert.AreSame(posts[0].author_obj, posts[1].author_obj);

			Assert.AreEqual(posts[0].id, 1);
			Assert.AreEqual(posts[0].title, "post1");
			Assert.AreEqual(posts[0].author, 1);
			Assert.AreEqual(posts[0].author_obj.name, "Bill");

			Assert.AreEqual(posts[1].id, 2);
			Assert.AreEqual(posts[1].title, "post2");
			Assert.AreEqual(posts[1].author, 1);
			Assert.AreEqual(posts[1].author_obj.name, "Bill");

			Assert.AreEqual(posts[2].id, 3);
			Assert.AreEqual(posts[2].title, "post3");
			Assert.AreEqual(posts[2].author, 2);
			Assert.AreEqual(posts[2].author_obj.name, "Ted");
		}

		class AuthorPostRelator
		{

			/*
			 * In order to support OneToMany relationship mapping, we need to be able to 
			 * delay returning an LHS object until we've processed its many RHS objects
			 * 
			 * To support this, PetaPoco allows a relator callback to return null - indicating
			 * that the object isn't yet fully populated.  
			 * 
			 * In order to flush the final object, PetaPoco will call the relator function 
			 * one final time with all parameters set to null.	It only does this if the callback
			 * returned null at least once during the processing of the result set (this saves
			 * simple lamba mapping functions from having to deal with nulls).
			 * 
			 */
			public author current;
			public author MapIt(author a, post p)
			{
				// Terminating call.  Since we can return null from this function
				// we need to be ready for PetaPoco to callback later with null
				// parameters
				if (a == null)
					return current;

				// Is this the same author as the current one we're processing
				if (current != null && current.id == a.id)
				{
					// Yes, just add this post to the current author's collection of posts
					current.posts.Add(p);

					// Return null to indicate we're not done with this author yet
					return null;
				}

				// This is a different author to the current one, or this is the 
				// first time through and we don't have an author yet

				// Save the current author
				var prev = current;

				// Setup the new current author
				current = a;
				current.posts = new List<post>();
				current.posts.Add(p);

				// Return the now populated previous author (or null if first time through)
				return prev;
			}
		}

		[Test]
		public void OneToMany()
		{
			// Example of OneToMany mappings

			var authors = db.Fetch<author, post, author>(new AuthorPostRelator().MapIt,
				"SELECT * FROM authors LEFT JOIN posts ON posts.author = authors.id ORDER BY posts.id"
				);

			Assert.AreEqual(authors.Count, 2);
			Assert.AreEqual(authors[0].name, "Bill");
			Assert.AreEqual(authors[0].posts.Count, 2);
			Assert.AreEqual(authors[0].posts[0].title, "post1");
			Assert.AreEqual(authors[0].posts[1].title, "post2");
			Assert.AreEqual(authors[1].name, "Ted");
			Assert.AreEqual(authors[1].posts.Count, 1);
			Assert.AreEqual(authors[1].posts[0].title, "post3");
		}

		[Test]
		public void ManyToOne_Lambda()
		{
			// same as ManyToOne test case above, but uses a lambda method as the callback
			var authors = new Dictionary<long, author>();
			var posts = db.Fetch<post, author, post>(
				(p, a) =>
				{
					// Get existing author object
					author aExisting;
					if (authors.TryGetValue(a.id, out aExisting))
						a = aExisting;
					else
						authors.Add(a.id, a);

					// Wire up objects
					p.author_obj = a;
					return p;
				},
				"SELECT * FROM posts LEFT JOIN authors ON posts.author = authors.id ORDER BY posts.id"
				);


			Assert.AreEqual(posts.Count, 3);
			Assert.AreSame(posts[0].author_obj, posts[1].author_obj);
			Assert.AreEqual(posts[0].id, 1);
			Assert.AreEqual(posts[0].title, "post1");
			Assert.AreEqual(posts[0].author, 1);
			Assert.AreEqual(posts[0].author_obj.name, "Bill");
			Assert.AreEqual(posts[1].id, 2);
			Assert.AreEqual(posts[1].title, "post2");
			Assert.AreEqual(posts[1].author, 1);
			Assert.AreEqual(posts[1].author_obj.name, "Bill");
			Assert.AreEqual(posts[2].id, 3);
			Assert.AreEqual(posts[2].title, "post3");
			Assert.AreEqual(posts[2].author, 2);
			Assert.AreEqual(posts[2].author_obj.name, "Ted");
		}


	}

}
