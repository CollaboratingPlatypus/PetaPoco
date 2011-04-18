using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Dynamic;

namespace PetaPoco.DevBed
{

	// Attributed not-so-true poco
	[TableNameAttribute("petapoco")]
	[PrimaryKeyAttribute("id")]
	[ExplicitColumnsAttribute]
	class deco
	{
		[ColumnAttribute]
		public long id { get; set; }
		[ColumnAttribute]
		public string title { get; set; }
		[ColumnAttribute]
		public bool draft { get; set; }
		[ColumnAttribute]
		public DateTime date_created { get; set; }
		[ColumnAttribute]
		public DateTime? date_edited { get; set; }
		[ColumnAttribute]
		public string content { get; set; }
	}


	class Program
	{

		static void Main(string[] args)
		{
			var db = new Database("mysql");

			var a=db.Single<dynamic>("SELECT * FROM articles WHERE article_id=20");
			a.title="Jab2 - Site Management";
			//a.title="New Title";
			db.Update("articles", "article_id", a);


			var l = db.Query<dynamic>("SELECT * FROM articles");
			foreach (var a2 in l)
			{
				Console.WriteLine("{0} = {1}", a2.article_id, a2.title);
			}

		}
	}
}
