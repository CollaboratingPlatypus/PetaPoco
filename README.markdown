### Version 6 - Netstandard 2.0

Read more about this [update](https://github.com/CollaboratingPlatypus/PetaPoco/wiki/V6NetStandard2-0)

|Master|Development|Nuget|
|:-----|:----------|:----|
|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/master?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/development?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/development)|[![Nuget Downloads](https://buildstats.info/nuget/PetaPoco.NetStandard)](https://www.nuget.org/packages/PetaPoco.NetStandard/)|

### Version 5 - Legacy

|Master|Development|Nuget|Nuget Core|Nuget Core Compiled|
|:-----|:----------|:----|:---------|:------------------|
|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/master-v5?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/master-v5)|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/development-v5?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/development-v5)|[![Nuget Downloads](https://buildstats.info/nuget/PetaPoco)](https://www.nuget.org/packages/PetaPoco/)|[![Nuget Downloads core](https://buildstats.info/nuget/PetaPoco.Core)](https://www.nuget.org/packages/PetaPoco.Core)|[![Nuget Downloads core](https://buildstats.info/nuget/PetaPoco.Core.Compiled)](https://www.nuget.org/packages/PetaPoco.Core.Compiled)|

<img align="right" alt="PetaPoco Logo" width="128" src="https://raw.githubusercontent.com/CollaboratingPlatypus/PetaPoco/master/Media/Logo2/PetaPocoLogo2_256.png">

# Welcome to the official PetaPoco repository

Originally the brainchild of [Brad Robinson],

## PetaPoco is a tiny, fast, single-file micro-ORM for .NET and Mono

* Like [Dapper], it's fast because it uses dynamic method generation (MSIL) to assign column values to properties
* Like [Massive] it's available as single file that you easily add to any project or complied.
* Like [Massive], it now also supports dynamic Expandos too - [read more](http://www.toptensoftware.com/blog/posts/104-PetaPoco-Not-So-Poco-or-adding-support-for-dynamic)
* Unlike [Massive] it works with strongly typed [POCO]'s
* Like [ActiveRecord], it supports a close relationship between object and database table
* Like [SubSonic], it supports generation of poco classes with T4 templates

## Features at a Glance

* Tiny, and absolutely no dependencies!
* Works with strictly undecorated POCOs, or attributed almost-POCOs.
* Easy to configure and includes [fluent configuration] out of the box.
* Helper methods for Insert/Delete/Update/Save and IsNew
* Paged requests automatically work out total record count and fetch a specific page.
* Easy transaction support.
* Better parameter replacement support, including grabbing named parameters from object properties.
* Great performance by eliminating Linq and fast property assignment with DynamicMethod generation.
* The query language is good ole SQL.
* Includes a low friction SQL builder class that makes writing inline SQL *much* easier.
* Includes T4 templates to automatically generate POCO classes for you.
* Hooks for logging exceptions, installing value converters and mapping columns to properties without attributes.
* Works with SQL Server, SQL Server CE, MS Access, SQLite, MySQL, MariaDB, Firebird, and PostgreSQL. (Oracle supported but does not have integration tests).
* Works under .NET 4.0/4.5/4.6 or Mono 2.8 and later.
* Has [Xunit] unit tests.
* Has supported DBs integration tests.
* OpenSource (Apache License)
* All of this in about 1,500 lines of code

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

Plus much much [more](https://github.com/CollaboratingPlatypus/PetaPoco/wiki).

## Documentation

For configuration, code examples and other general information [See the docs]

## Download

PetaPoco is available from:

* **NuGet Peta** - <https://www.nuget.org/packages/PetaPoco/> - A single file include with [T4 text templates](https://msdn.microsoft.com/en-us/library/bb126445.aspx) to automatically build your pocos for you.
* **NuGet Peta Core** - <https://www.nuget.org/packages/PetaPoco.Core/> - A single file include.
* **NuGet Peta Core Compiled** - <https://www.nuget.org/packages/PetaPoco.Core.Compiled/> - A compiled version which your project references.

[Brad Robinson]:http://www.toptensoftware.com/
[Massive]:https://github.com/FransBouma/Massive
[Dapper]:https://github.com/StackExchange/dapper-dot-net
[SubSonic]:http://subsonic.github.io/
[ActiveRecord]:http://guides.rubyonrails.org/active_record_basics.html
[POCO]:http://en.wikipedia.org/wiki/Plain_Old_CLR_Object
[CodingHorror]:http://www.subsonicproject.com/docs/CodingHorror
[XUnit]:https://github.com/xunit/xunit
[See the docs]:https://github.com/CollaboratingPlatypus/PetaPoco/wiki
[the contribution guide]:./contributing.md
[Contributing to Open Source on GitHub]:https://guides.github.com/activities/contributing-to-open-source/
[we have a special place to honour them]:./honourRoll.md
[fluent configuration]:https://github.com/CollaboratingPlatypus/PetaPoco/wiki/Fluent-Configuration

## Contributing

PetaPoco welcomes input from the community. After all, what is a product without users? If you’d like to contribute, please take the time to read [the contribution guide]. We would also suggest you have a quick read of [Contributing to Open Source on GitHub].

## Contributions Honour Roll

A product like PetaPoco isn't the effort of one person, but rather a combined effort of many. For those individuals who rise above and beyond [we have a special place to honour them].
