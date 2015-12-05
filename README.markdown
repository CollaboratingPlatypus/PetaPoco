|Master|Development|Nuget|Nuget Core|Nuget Core Compiled|
|:-----|:----------|:----|:---------|:------------------|
|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/master?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/1vodaox1reremsvj/branch/development?svg=true)](https://ci.appveyor.com/project/collaboratingplatypus/petapoco/branch/development)|[![Nuget Downloads](https://img.shields.io/nuget/dt/PetaPoco.svg)](https://img.shields.io/nuget/dt/PetaPoco.svg)|[![Nuget Downloads core](https://img.shields.io/nuget/dt/PetaPoco.Core.svg)](https://img.shields.io/nuget/dt/PetaPoco.Core.svg)|[![Nuget Downloads core](https://img.shields.io/nuget/dt/PetaPoco.Core.Compiled.svg)](https://img.shields.io/nuget/dt/PetaPoco.Core.Compiled.svg)

----

Originally the brainchild of [Brad Robinson],

## PetaPoco is a tiny, fast, single-file micro-ORM for .NET and Mono.

* Like [Massive] it's a single file that you easily add to any project
* Unlike [Massive] it works with strongly typed [POCO]'s
* Like [Massive], it now also supports dynamic Expandos too - [read more](http://www.toptensoftware.com/blog/posts/104-PetaPoco-Not-So-Poco-or-adding-support-for-dynamic)
* Like [ActiveRecord], it supports a close relationship between object and database table
* Like [SubSonic], it supports generation of poco classes with T4 templates
* Like [Dapper], it's fast because it uses dynamic method generation (MSIL) to assign column values to properties

---
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
* Works under .NET 4.0/4.5/4.6 or Mono 2.8 and later.
* Experimental support for `dynamic` under .NET 4.0 and Mono 2.8
* [Xunit] unit tests.
* OpenSource (Apache License)
* All of this in about 1,500 lines of code

---
## Documentation

For configuration, code examples and other general information [See the docs]

---
## Contributing

PetaPoco welcomes input form the community. After all, what is a product without users? If you’d like to contribute, please take the time to read [the contribution guide]. We would also suggest you have a quick read of [Contributing to Open Source on GitHub].

---
## Contributions Honour Roll

A product like PetaPoco isn't the effort of one person, but rather a combined effort of many. For those individuals who rise above and beyond [we have a special place to honour them].

---
## Download

PetaPoco is available from:

* NuGet Peta - <https://www.nuget.org/packages/PetaPoco/>
* NuGet Peta Core - <https://www.nuget.org/packages/PetaPoco.Core/>
* NuGet Peta Core Compiled - <https://www.nuget.org/packages/PetaPoco.Core.Compiled/>

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