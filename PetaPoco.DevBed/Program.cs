using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

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
			var page = db.Page<article>(5,5,"ORDER BY article_id");
			foreach (var a in page.Items)
			{
				Console.WriteLine("{0}\t{1}", a.article_id, a.title);
			}
		}
	}
}
