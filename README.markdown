# PetaPoco #

<h2 class="tagline">A tiny ORM-ish thing for your POCOs</h2>

PetaPoco is a tiny .NET data access layer inspired by Rob Conery's [Massive](http://blog.wekeroad.com/helpy-stuff/and-i-shall-call-it-massive) 
project but for use with non-dynamic [POCO](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object) objects.  It came about because I needed a 
data-access layer that was tiny, fast, easy to use and could run on .NET 3.5 and/or Mono 2.6 and later.  (ie: no support for dynamic).  Rob's 
claim of Massive being only 400 lines of code intruiged me and I wondered if something similar could be done without dynamics.

So, what's with the name?  Well if Massive is massive, this is peta-massive (it's about twice the size 
after all) and it works with POCOs ... so PetaPoco!!

## Show Me the Code! ##

These examples start out more verbose than they need to be but become less so as more features are 
introduced... make sure you read to the bottom for the full experience.  I've explicitly referenced the PetaPoco 
namespace to make it obvious what comes from where but in reality you'd probably chuck in a `using PetaPoco;`.

Also, all of these examples have been hand-typed and never compiled.  There are probably
typos.  If so, please [let me know](http://www.toptensoftware.com/contact).

### Running Queries ###

First define your POCOs:

	{{C#}}
	// Represents a record in the "articles" table
	public class article
	{
		public long article_id { get; set; }
		public string title { get; set; }
		public DateTime date_created { get; set; }
		public bool draft { get; set; }
		public string content { get; set; }
	}

Next, create a `PetaPoco.Database` and run the query:

	{{C#}}
	// Create a PetaPoco database object
	var db=new PetaPoco.Database("connectionStringName");

	// Show all articles	
	foreach (var a in db.Query<article>("SELECT * FROM articles"))
	{
		Console.WriteLine("{0} - {1}", a.article_id, a.title);
	}
	
To query a scalar:

	{{C#}}
	long count=db.ExecuteScalar<long>("SELECT Count(*) FROM articles");
	
Or, to get a single record:

	{{C#}}
	var a = db.SingleOrDefault<article>("SELECT * FROM articles WHERE article_id=@0", 123));
	
### Query vs Fetch ###

The Database class has two methods for retrieving records `Query` and `Fetch`.  These are pretty
much identical except Fetch returns a List<> of POCO's whereas Query uses `yield return` to iterate
over the results without loading the whole set into memory.

### Non-query Commands ###

To execute non-query commands, use the Execute method

	{{C#}}
	db.Execute("DELETE FROM articles WHERE draft<>0");
	
### Inserts, Updates and Deletes ###

PetaPoco has helpers for insert, update and delete operations.

To insert a record, you need to specify the table and its primary key:

	{{C#}}
	// Create the article
	var a=new article();
	a.title="My new article";
	a.content="PetaPoco was here";
	a.date_created=DateTime.UtcNow;
	
	// Insert it
	db.Insert("articles", "article_id", a);
	
	// by now a.article_id will have the id of the new article
	
Updates are similar:

	{{C#}}
	// Get a record
	var a=db.SingleOrDefault<article>("SELECT * FROM articles WHERE article_id=@0", 123);
	
	// Change it
	a.content="PetaPoco was here again";
	
	// Save it
	db.Update("articles", "article_id", a);
	
Or you can pass an anonymous type update a subset of fields.  In this case only article's title field will be updated.

	{{C#}}
	db.Update("articles", "article_id", new { title="New title" }, 123);
	
To delete:

	{{C#}}
	// Delete an article extracting the primary key from a record
	db.Delete("articles", "article_id", a);
	
	// Or if you already have the ID elsewhere
	db.Delete("articles", "article_id", null, 123);


### Decorating Your POCOs

In the above examples, it's a pain to have to specify the table name and primary key all over the place,
so you can attach this info to your POCO:

	{{C#}}
	// Represents a record in the "articles" table
	[PetaPoco.TableName("articles")]
	[PetaPoco.PrimaryKey("article_id")]
	public class article
	{
		public long article_id { get; set; }
		public string title { get; set; }
		public DateTime date_created { get; set; }
		public bool draft { get; set; }
		public string content { get; set; }
	}

Now inserts, updates and deletes get simplified to this:

	{{C#}}
	// Insert a record
	var a=new article();
	a.title="My new article";
	a.content="PetaPoco was here";
	a.date_created=DateTime.UtcNow;
	db.Insert(a);
	
	// Update it
	a.content="Blah blah";
	db.Update(a);
	
	// Delete it
	db.Delete(a)
	
You can also tell it to ignore certain fields:

	{{C#}}
	public class article
	{
		[PetaPoco.Ignore]
		public long SomeCalculatedFieldPerhaps
		{ 
			get; set; 
		}
	}

### Hey! Wait a minute. Aren't there already standard attributes for decorating a POCO's database info?

Well I could use them but there are so few that PetaPoco supports that I didn't want to cause confusion over what it could do.

### Transactions

Transactions are pretty simple:

	{{C#}}
	using (var scope=db.Transaction)
	{
		// Do transacted updates here
		
		// Commit
		scope.Complete();
	}
	
	
Transactions can be nested, so you can call out to other methods with their own nested transaction scopes
and the whole lot will be wrapped up in a single transaction.  So long as all nested transcaction scopes 
are completed the entire root level transaction is committed, otherwise everything is rolled back.

Note: for transactions to work, all operations need to use the same instance of the PetaPoco database 
object.  So you'll probably want to use a per-http request, or per-thread IOC container to serve up a shared
instance of this object.  Personally [StructureMap](http://structuremap.net) is my favourite for this.

### But where's the LINQ stuff?

There isn't any.  I've used Linq with Subsonic for a long time now and more and more I find myself descending
into [CodingHorror](http://subsonicproject.com/docs/CodingHorror) for things that:

* can't be done in Linq easily
* work in .NET but not under Mono (especially Mono 2.6)
* don't perform efficiently.  Eg: Subsonic's activerecord.SingleOrDefault(x=x.id==123) seems to be about 20x
slower than CodingHorror. (See [here](https://github.com/subsonic/SubSonic-3.0/issues/258))

Now that I've got CodingHorror all over the place it bugs me that half the code is Linq and half is SQL.

Also, I've realized that for me the most annoying thing about SQL directly in the code is not the fact that it's 
SQL but that it's nasty to format nicely and to build up those SQL strings.

So...

### PetaPoco's SQL Builder

There's been plenty of attempts at building fluent type API's for building SQL.  This is my version and it's really
basic.  

The point of this is to make formatting the SQL strings easy and to use proper parameter replacements
to protect from SQL injection. This is not an attempt to ensure the SQL is syntactically correct, nor is it
trying to hold anyone's hand with intellisense.

Here's its most basic form:

	{{C#}}
	var id=123;
	var a=db.Query<article>(new PetaPoco.Sql()
		.Append("SELECT * FROM articles")
		.Append("WHERE article_id=@0", id)
	)

Big deal right?  Well what's cool about this is that the parameter indicies are specific to each `.Append` call:

	{{C#}}
	var id=123;
	var a=db.Query<article>(new PetaPoco.Sql()
		.Append("SELECT * FROM articles")
		.Append("WHERE article_id=@0", id)
		.Append("AND date_created<@0", DateTime.UtcNow)
	)

You can also conditionally build SQL.  

	{{C#}}
	var id=123;
	var sql=new PetaPoco.Sql()
		.Append("SELECT * FROM articles")
		.Append("WHERE article_id=@0", id);
		
	if (start_date.HasValue)
		sql.Append("AND date_created>=@0", start_date.Value);
		
	if (end_date.HasValue)
		sql.Append("AND date_created<=@0", end_date.Value);
		
	var a=db.Query<article>(sql)

Note that each append call uses parameter @0?  PetaPoco builds the full list of arguments and 
updates the parameter indices internally for you.

You can also use named parameters and it will look for an appropriately named property on
any of the passed arguments

	{{C#}}
	sql.Append("AND date_created>=@start AND date_created<=@end", 
					new 
					{ 
						start=DateTime.UtcNow.AddDays(-2), 
						end=DateTime.UtcNow 
					}
				);
				
With both numbered and named parameters, if any of the parameters can't be resolved 
an exception is thrown.

There are also methods for building common SQL stuff:

	var sql=new PetaPoco.Sql()
				.Select("*")
				.From("articles")
				.Where("date_created < @0", DateTime.UtcNow)
				.OrderBy("date_created DESC");


## That's it.

This was knocked together over about a 24-hour period and I'm yet to use it in a real project, but I'm about to. 
I expect it will be updated reasonably regularly over the coming weeks so check back often.

Let me know what you think - comments, suggestions and criticisms and welcome [here](http://toptensoftware.com/contact).