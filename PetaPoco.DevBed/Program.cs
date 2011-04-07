using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace PetaPoco.DevBed
{

	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id")]
	[ExplicitColumns]
	class deco
	{
		[Column]
		public long id { get; set; }
		[Column]
		public string title { get; set; }
		[Column]
		public bool draft { get; set; }
		[Column]
		public DateTime date_created { get; set; }
		[Column]
		public DateTime? date_edited { get; set; }
		[Column]
		public string content { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			var db=new PetaPoco.Database("oracle");
			db.Execute("SELECT * FROM BLAH");

		}
	}
}
