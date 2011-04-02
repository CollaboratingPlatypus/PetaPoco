# PetaPoco #

<h2 class="tagline">A tiny ORM-ish thing for your POCOs</h2>

PetaPoco is a tiny .NET data access layer inspired by Rob Conery's [Massive](http://blog.wekeroad.com/helpy-stuff/and-i-shall-call-it-massive) 
project but for use with non-dynamic [POCO](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object) objects.  It came about because I was finding
many of my projects that used SubSonic/Linq were slow or becoming mixed bags of Linq and [CodingHorror](http://www.subsonicproject.com/docs/CodingHorror).

I needed a data acess layer that was:

* tiny
* fast
* easy to use and similar to SubSonic
* could run on .NET 3.5 and/or Mono 2.6 (ie: no support for dynamic).  

Rob's claim of Massive being only 400 lines of code intruiged me and I wondered if something similar could be done without dynamics.

So, what's with the name?  Well if Massive is massive, this is "Peta" massive (at about 1,200 lines it's triple the size after all) and since it 
works with "Poco"s ... "PetaPoco" seemed like a fun name!!


See here - <http://www.toptensoftware.com/petapoco> - for full details.
