using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PetaPoco;

namespace PetaPoco.Tests
{
	public class MultiPocoTests : AssertionHelper
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
			Expect(posts.Count, Is.EqualTo(3));

			Expect(posts[0].id, Is.EqualTo(1));
			Expect(posts[0].title, Is.EqualTo("post1"));
			Expect(posts[0].author, Is.EqualTo(1));
			Expect(posts[0].author_obj.name, Is.EqualTo("Bill"));

			Expect(posts[1].id, Is.EqualTo(2));
			Expect(posts[1].title, Is.EqualTo("post2"));
			Expect(posts[1].author, Is.EqualTo(1));
			Expect(posts[1].author_obj.name, Is.EqualTo("Bill"));

			Expect(posts[2].id, Is.EqualTo(3));
			Expect(posts[2].title, Is.EqualTo("post3"));
			Expect(posts[2].author, Is.EqualTo(2));
			Expect(posts[2].author_obj.name, Is.EqualTo("Ted"));
		}						

	}

}
