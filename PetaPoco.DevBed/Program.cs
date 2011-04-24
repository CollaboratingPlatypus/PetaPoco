using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Dynamic;

namespace PetaPoco.DevBed
{

	class Program
	{

		static void Main(string[] args)
		{
			var db = new PetaPoco.Database("mysql");

			Console.WriteLine(db.SingleOrDefault<string>("SELECT title FROM articles WHERE article_id=@0", 270));
		}
	}
}
