---
title: "PetaPoco"
subtitle: "A tiny ORM-ish thing for your POCOs"
defaultSyntax: "cs"
merge:
    before: "header.page"
---

PetaPoco is a tiny, fast, single-file micro-ORM for .NET and Mono.

* Like [Massive] it's a single file that you easily add to any project
* Unlike [Massive] it works with strongly typed [POCO]'s
* Like [Massive], it now also supports dynamic Expandos too - [read more](http://www.toptensoftware.com/blog/posts/104-PetaPoco-Not-So-Poco-or-adding-support-for-dynamic)
* Like [ActiveRecord], it supports a close relationship between object and database table
* Like [SubSonic], it supports generation of poco classes with T4 templates
* Like [Dapper], it's fast because it uses dynamic method generation (MSIL) to assign column values to properties

[Massive]:http://rob.conery.io/2011/02/16/massive-400-lines-of-data-access-happiness/
[Dapper]:http://code.google.com/p/dapper-dot-net/
[SubSonic]:http://www.subsonicproject.com
[ActiveRecord]:http://ar.rubyonrails.org/
[POCO]:http://en.wikipedia.org/wiki/Plain_Old_CLR_Object
[CodingHorror]:http://www.subsonicproject.com/docs/CodingHorror

## Background

PetaPoco was original inspired by Rob Conery's [Massive] project but for use with non-dynamic [POCO] objects.  It came about because I was finding many of my projects that used SubSonic/Linq were slow or becoming mixed bags of Linq and [CodingHorror].

I needed a data acess layer that was tiny, fast, easy to use and could run on .NET 3.5 and/or Mono 2.6 (ie: no support for dynamic expandos).  Rob's claim of Massive being only 400 lines of code intruiged me and I wondered if something similar could be done without dynamics.

So, what's with the name?  Well if Massive is massive, this is "Peta" massive (it's now over 1,500 lines after all) and since it works with "Poco"s ... "PetaPoco" seemed like a fun name!!

PetaPoco's line count has grown to more than I originally hoped - it's not the tiny 400 lines of Massive. But check out what it can do ... it packs a lot of punch for it's size.

## Features at a Glance

* Tiny, no dependencies... a single C# file you can easily add to any project.
* Works with strictly undecorated POCOs, or attributed almost-POCOs.
* Helper methods for Insert/Delete/Update/Save and IsNew
* Paged requests automatically work out total record count and fetch a specific page.
* Easy transaction support.
* Better parameter replacement support, including grabbing named parameters from object properties.
* Great performance by eliminating Linq and fast property assignment with DynamicMethod generation.
* Includes T4 templates to automatically generate POCO classes for you.
* The query language is SQL... no weird fluent or Linq syntaxes (yes, matter of opinion)
* Includes a low friction SQL builder class that makes writing inline SQL *much* easier.
* Hooks for logging exceptions, installing value converters and mapping columns to properties without attributes.
* Works with SQL Server, SQL Server CE, MySQL, PostgreSQL and Oracle.
* Works under .NET 4.0 or Mono 2.8 and later.
* Experimental support for `dynamic` under .NET 4.0 and Mono 2.8
* NUnit unit tests.
* OpenSource (Apache License)
* All of this in about 1,500 lines of code

## Download ##

PetaPoco is available from:

* NuGet - <http://nuget.org/List/Packages/PetaPoco>
* GitHub - <https://github.com/toptensoftware/petapoco>

## Show Me the Code! ##

These examples start out more verbose than they need to be but become less so as more features are 
introduced... make sure you read to the bottom for the full experience.  I've explicitly referenced the PetaPoco 
namespace to make it obvious what comes from where but in reality you'd probably chuck in a `using PetaPoco;`.

Also, all of these examples have been hand-typed and never compiled.  There are probably
typos.  If so, please [let me know](http://www.toptensoftware.com/contact).

### No Assembly ###

PetaPoco is supplied as a single file - [PetaPoco.cs](https://github.com/toptensoftware/PetaPoco/blob/master/PetaPoco/PetaPoco.cs).  With no dependencies other than
what's in the GAC, just add this file to your project and you're set to go...

### Running Queries ###

First define your POCOs:

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

	// Create a PetaPoco database object
	var db=new PetaPoco.Database("connectionStringName");

	// Show all articles	
	foreach (var a in db.Query<article>("SELECT * FROM articles"))
	{
		Console.WriteLine("{0} - {1}", a.article_id, a.title);
	}
	
To query a scalar:

	long count=db.ExecuteScalar<long>("SELECT Count(*) FROM articles");
	
Or, to get a single record:

	var a = db.SingleOrDefault<article>("SELECT * FROM articles WHERE article_id=@0", 123));
	

### Paged Fetches

PetaPoco can automatically perform paged requests.

	
	var result=db.Page<article>(1, 20, // <-- page number and items per page
			"SELECT * FROM articles WHERE category=@0 ORDER BY date_posted DESC", "coolstuff");

In return you'll get a PagedFetch object:

	public class Page<T> where T:new()
	{
		public long CurrentPage { get; set; }
		public long ItemsPerPage { get; set; }
		public long TotalPages { get; set; }
		public long TotalItems { get; set; }
		public List<T> Items { get; set; }
	}

Behind the scenes, PetaPoco does the following:

1. Synthesizes and executes a query to retrieve the total number of matching records
2. Modifies your original query to request just a subset of the entire record set

You now have everything to display a page of data and a pager control all wrapped up in one handy 
little object!


### Query vs Fetch ###

The Database class has two methods for retrieving records `Query` and `Fetch`.  These are pretty
much identical except Fetch returns a List<> of POCO's whereas Query uses `yield return` to iterate
over the results without loading the whole set into memory.

### Non-query Commands ###

To execute non-query commands, use the Execute method

	db.Execute("DELETE FROM articles WHERE draft<>0");
	

### Inserts, Updates and Deletes ###

PetaPoco has helpers for insert, update and delete operations.

To insert a record, you need to specify the table and its primary key:

	// Create the article
	var a=new article();
	a.title="My new article";
	a.content="PetaPoco was here";
	a.date_created=DateTime.UtcNow;
	
	// Insert it
	db.Insert("articles", "article_id", a);
	
	// by now a.article_id will have the id of the new article
	
Updates are similar:

	// Get a record
	var a=db.SingleOrDefault<article>("SELECT * FROM articles WHERE article_id=@0", 123);
	
	// Change it
	a.content="PetaPoco was here again";
	
	// Save it
	db.Update("articles", "article_id", a);
	
Or you can pass an anonymous type to update a subset of fields.  In this case only the article's title field will be updated.

	db.Update("articles", "article_id", new { title="New title" }, 123);
	
To delete:

	// Delete an article extracting the primary key from a record
	db.Delete("articles", "article_id", a);
	
	// Or if you already have the ID elsewhere
	db.Delete("articles", "article_id", null, 123);


### Decorating Your POCOs

In the above examples, it's a pain to have to specify the table name and primary key all over the place,
so you can attach this info to your POCO:

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
	db.Delete(a);
	
There are also other overloads for Update and Delete:


	// Delete an article
	db.Delete<article>("WHERE article_id=@0", 123);
	
	// Update an article
	db.Update<article>("SET title=@0 WHERE article_id=@1", "New Title", 123);
	
	
You can also tell it to ignore certain fields:

	public class article
	{
		[PetaPoco.Ignore]
		public long SomeCalculatedFieldPerhaps
		{ 
			get; set; 
		}
	}

Or, perhaps you'd like to be a little more explicit. Rather than automatically mapping all columns you can
use the ExplicitColumns attribute on the class and the Column to indicate just those columns that should be
mapped.

	// Represents a record in the "articles" table
	[PetaPoco.TableName("articles")]
	[PetaPoco.PrimaryKey("article_id")]
	[PetaPoco.ExplicitColumns]
	public class article
	{
		[PetaPoco.Column] public long article_id { get; set; }
		[PetaPoco.Column] public string title { get; set; }
		[PetaPoco.Column] public DateTime date_created { get; set; }
		[PetaPoco.Column] public bool draft { get; set; }
		[PetaPoco.Column] public string content { get; set; }
	}

This works great with partial classes. Put all your table binding stuff in one .cs file and calculated and 
other useful properties can be added in a separate file with out thinking about the data layer).

### Hey! Aren't there already standard attributes for decorating a POCO's database info?

Well I could use them but there are so few that PetaPoco supports that I didn't want to cause confusion over what it could do.

### Hey! Wait a minute... they're not POCO objects!

Your right, the attributes really do break the strict concept of [POCO](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object), 
but if you can live with that they really do make working with PetaPoco easy.

### T4 Template

Writing all those POCO objects can soon get tedious and error prone... so PetaPoco includes a [T4 template](http://www.hanselman.com/blog/T4TextTemplateTransformationToolkitCodeGenerationBestKeptVisualStudioSecret.aspx) 
that can automatically write classes for all the tables in your your SQL Server, SQL Server CE, MySQL, PostgreSQL or Oracle database.

Using the T4 template is pretty simple.  The git repository includes three files (The NuGet package adds 
these to your project automatically in the folder `\Models\Generated`).

* PetaPoco.Core.ttinclude - includes all the helper routines for reading the DB schema
* PetaPoco.Generator.ttinclude - the actual template that defines what's generated
* Database.tt - the template itself that includes various settings and includes the two other ttinclude files.

A typical Database.tt file looks like this:

	<#@ include file="PetaPoco.Core.ttinclude" #>
	<#
		// Settings
		ConnectionStringName = "jab";
		Namespace = ConnectionStringName;
		DatabaseName = ConnectionStringName;
		string RepoName = DatabaseName + "DB";
		bool GenerateOperations = true;
	    
		// Load tables
		var tables = LoadTables();
		
	#>
	<#@ include file="PetaPoco.Generator.ttinclude" #>

To use the template:

1. Add the three files to you C# project
2. Make sure you have a connection string and provider name set in your app.config or web.config file
3. Edit ConnectionStringName property in Records.tt (ie: change it from "jab" to the name of your connection string)
4. Save Database.tt.  

All going well Database.cs should be generated with POCO objects representing all the tables in your database. To get 
the project to build you'll also need to add PetaPoco.cs to your project and ensure it is set to compile (NuGet does 
this for you) .

The template is based on the [SubSonic](http://subsonicproject.com) template.  If you're familiar with its
ActiveRecord templates you'll find PetaPoco's template very similar. 

### Automatic Select clauses

When using PetaPoco, most queries start with "SELECT * FROM table".  Given that we can now grab the table 
name from the POCO object using the TableName attribute, there's no reason we can't automatically
generate this part of the select statement.

If you run a query that doesn't start with "SELECT", PetaPoco will automatically put it in. So this:

	// Get a record
	var a=db.SingleOrDefault<article>("SELECT * FROM articles WHERE article_id=@0", 123);
	
can be shortened to this:

	// Get a record
	var a=db.SingleOrDefault<article>("WHERE article_id=@0", 123);
	
PetaPoco doesn't actually generate "SELECT *"... rather it picks the column names of the POCO
and just queries for those columns.


### IsNew and Save Methods

Sometimes you have a POCO and you want to know if it's already in the database.  Since we have the primary key all we need to do is check if that property has been set to something other than the default value.

So to test if a record is new:

	// Is this a new record	
	if (db.IsNew(a))
	{
		// Yes it is...
	}

And related, there's a Save method that will work out whether to Insert or Update

	// Save a new or existing record
	db.Save(a);


### Transactions

Transactions are pretty simple:

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

	var id=123;
	var a=db.Query<article>(PetaPoco.Sql.Builder
		.Append("SELECT * FROM articles")
		.Append("WHERE article_id=@0", id)
	)

Big deal right?  Well what's cool about this is that the parameter indicies are specific to each `.Append` call:

	var id=123;
	var a=db.Query<article>(PetaPoco.Sql.Builder
		.Append("SELECT * FROM articles")
		.Append("WHERE article_id=@0", id)
		.Append("AND date_created<@0", DateTime.UtcNow)
	)

You can also conditionally build SQL.  

	var id=123;
	var sql=PetaPoco.Sql.Builder
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

	var sql=PetaPoco.Sql.Builder()
				.Select("*")
				.From("articles")
				.Where("date_created < @0", DateTime.UtcNow)
				.OrderBy("date_created DESC");

### SQL Command Tracking

Sometime it's useful to be able to see what SQL was just executed.  PetaPoco exposes these three properties:

* LastSQL - pretty obvious
* LastArgs - an object[] array of all arguments passed
* LastCommand - a string that shows the SQL and the arguments

Watching the LastCommand property in the debugger makes it easy to see what just happened!

### OnException Handler Routine

PetaPoco wraps all SQL command invocations in try/catch statements. Any exceptions are passed
to the virtual OnException method.  By logging these exceptions (or setting a breakpoint on this method)
you can easily track where an when there are problems with your SQL.


## More

The covers most of the basics of working with PetaPoco, but for more please [read these blog posts about PetaPoco](/blog/tags/petapoco).
