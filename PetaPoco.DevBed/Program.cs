using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PetaPoco.DevBed
{
	[TableName("articles2")]
	[PrimaryKey("article_id")]
	[ExplicitColumns]
    public partial class article
    {
        [Column] public long article_id { get; set; }
        [Column] public long site_id { get; set; }
        [Column] public long user_id { get; set; }
        [Column] public DateTime? date_created { get; set; }
        [Column] public string title { get; set; }
        [Column] public string content { get; set; }
        [Column] public bool draft { get; set; }
//        [Column] public long local_article_id { get; set; }
        [Column] public long? wip_article_id { get; set; }
	}


	class Program
	{
		static void Main(string[] args)
		{

			
			var db=new PetaPoco.Database("mysql");

			/*
			var l1 = db.FetchIL<article>("ORDER BY article_id");
			var l2 = db.Fetch<article>("ORDER BY article_id");


			var sw2 = new System.Diagnostics.Stopwatch();
			sw2.Start();
			for (int i = 0; i < 100; i++)
			{
				l2 = db.Fetch<article>("ORDER BY article_id");
			}
			sw2.Stop();

			var sw1 = new System.Diagnostics.Stopwatch();
			sw1.Start();
			for (int i = 0; i < 100; i++)
			{
				l1 = db.FetchIL<article>("ORDER BY article_id");
			}
			sw1.Stop();

			Console.WriteLine("MSIL:      {0}", sw1.ElapsedTicks);
			Console.WriteLine("Reflection:{0}", sw2.ElapsedTicks);
			Console.WriteLine("Percent:   {0}", (double)(sw2.ElapsedTicks - sw1.ElapsedTicks)*100/(double)(sw2.ElapsedTicks));
			 */

			foreach (var a in db.Fetch<article>("ORDER BY article_id"))
			{
				Console.WriteLine("{0}\t{1}", a.article_id, a.title);
			}
		}
	}
}
