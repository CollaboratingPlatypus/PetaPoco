<img align="right" alt="PetaPoco Logo" width="128" src="https://raw.githubusercontent.com/CollaboratingPlatypus/PetaPoco/master/Media/Logo2/PetaPocoLogo2_256.png">

# Welcome to the official PetaPoco repository

Originally the brainchild of [Brad Robinson].

PetaPoco is maintained and extended by Wade Baglin (@pleb) and Aaron Sherber (@asherber).

## Version 6 - Netstandard 2.0+, 4.0, 4.5+

Read more about the [v6 update](https://github.com/CollaboratingPlatypus/PetaPoco/wiki/V6NetStandard2-0)

PetaPoco is available from: **NuGet [PetaPoco.Compiled](https://www.nuget.org/packages/PetaPoco.Compiled)**

|Master|Development|Nuget|
|:-----|:----------|:----|
|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/master?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/development?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/development)|[![Nuget Downloads](https://buildstats.info/nuget/PetaPoco.Compiled)](https://www.nuget.org/packages/PetaPoco.Compiled/)|

### Documentation

For configuration documentation, code examples, and other general usage information, [see the docs].

### Add-ons

* [PetaPoco.SqlKata](https://github.com/asherber/PetaPoco.SqlKata) lets you use the powerful query builder [SqlKata](https://sqlkata.com) to [build SQL queries](https://github.com/CollaboratingPlatypus/PetaPoco/wiki/Building-SQL-Queries) for PetaPoco.
* [StaTypPocoQueries.PetaPoco](https://github.com/asherber/StaTypPocoQueries.PetaPoco) provides the ability to use some simple, strongly typed, Intellisensed LINQ expressions in your queries.
* [PetaPoco.DBEntityGenerator](https://github.com/hippasus/PetaPoco.DBEntityGenerator) lets you generate Pocos from your DB (T4 template replacement).

## Version 5 - Legacy

|Nuget (Single file)|Nuget Core (+t4 templates)|Nuget Core Compiled (dll)|
|:----|:---------|:------------------|
|[![Nuget Downloads](https://buildstats.info/nuget/PetaPoco)](https://www.nuget.org/packages/PetaPoco/)|[![Nuget Downloads core](https://buildstats.info/nuget/PetaPoco.Core)](https://www.nuget.org/packages/PetaPoco.Core)|[![Nuget Downloads core](https://buildstats.info/nuget/PetaPoco.Core.Compiled)](https://www.nuget.org/packages/PetaPoco.Core.Compiled)|

---

## PetaPoco is a tiny & fast micro-ORM for .NET

* Like [Dapper], it's fast because it uses dynamic method generation (MSIL) to assign column values to properties
* Like [Massive], it now also supports dynamic Expandos too
* Like [ActiveRecord], it supports a close relationship between object and database table
* Like [SubSonic], it supports generation of poco classes with T4 templates (V5 only)
* Like [Massive], it's available as a single file that you easily add to any project or complied. (V5 only)

## Features at a Glance

* Tiny, and absolutely no dependencies!
* Asychronous (aka async) or synchronous: the choice is yours. (V6)
* Works with strictly undecorated POCOs, or attributed almost-POCOs.
* Easy to configure, including [fluent configuration] out of the box.
* Helper methods for Insert/Delete/Update/Save and IsNew
* Paged requests automatically work out the total record count and fetch a specific page.
* Easy transaction support.
* Better parameter replacement support, including grabbing named parameters from object properties.
* Great performance by eliminating Linq and fast property assignment with DynamicMethod generation.
* The query language is good'ole SQL.
* Includes a low friction SQL builder class that makes writing inline SQL *much* easier.
* Includes T4 templates to automatically generate POCO classes for you. (V5)
* Hooks for logging exceptions, installing value converters and mapping columns to properties without attributes.
* Works with SQL Server, SQL Server CE, MS Access, SQLite, MySQL, MariaDB, Firebird, and PostgreSQL. (Oracle is supported but does not have integration tests).
* Works under Net Standard 2.0, .NET 4.0/4.5+ or Mono 2.8 and later.
* Has [Xunit] unit tests.
* Has supported DBs integration tests.
* OpenSource (MIT License or Apache 2.0)

## Super easy use and configuration

Save an entity
```c#
db.Save(article);
db.Save(new Article { Title = "Super easy to use PetaPoco" });
db.Save("Articles", "Id", { Title = "Super easy to use PetaPoco", Id = Guid.New() });
```

Get an entity
```c#
var article = db.Single<Article>(123);
var article = db.Single<Article>("WHERE ArticleKey = @0", "ART-123");
```

Delete an entity
```c#
db.Delete(article);
db.Delete<Article>(123);
db.Delete("Articles", "Id", 123);
db.Delete("Articles", "ArticleKey", "ART-123");
```

Plus much, much [more](https://github.com/CollaboratingPlatypus/PetaPoco/wiki).

## Contributing

PetaPoco welcomes input from the community. After all, what is a product without users? If you would like to contribute, please take the time to read [the contribution guide]. We would also suggest you have a quick read of [Contributing to Open Source on GitHub].

## Contributions Honour Roll

PetaPoco is not the result of just one person's labour, but rather, the product of a combined effort by many. For those individuals who go above and beyond, [we have a special place to honour them].

[Brad Robinson]:https://www.toptensoftware.com/
[Massive]:https://github.com/FransBouma/Massive
[Dapper]:https://github.com/StackExchange/dapper-dot-net
[SubSonic]:https://subsonic.github.io/
[ActiveRecord]:https://guides.rubyonrails.org/active_record_basics.html
[POCO]:https://en.wikipedia.org/wiki/Plain_Old_CLR_Object
[CodingHorror]:https://web.archive.org/web/20160215051631/subsonicproject.com/docs/CodingHorror
[XUnit]:https://github.com/xunit/xunit
[see the docs]:https://github.com/CollaboratingPlatypus/PetaPoco/wiki
[the contribution guide]:./contributing.md
[Contributing to Open Source on GitHub]:https://guides.github.com/activities/contributing-to-open-source/
[we have a special place to honour them]:./honourRoll.md
[fluent configuration]:https://github.com/CollaboratingPlatypus/PetaPoco/wiki/Fluent-Configuration
