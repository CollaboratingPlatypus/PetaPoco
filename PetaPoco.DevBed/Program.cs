using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

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
		}
	}
}
