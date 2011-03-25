using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPoco.Tests
{
	// Non-decorated true poco
	class poco
	{
		public long id { get; set; }
		public string title { get; set; }
		public bool draft { get; set; }
		public DateTime date_created { get; set; }
		public DateTime? date_edited { get; set; }
		public string content { get; set; }
	}


	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id")]
	[ExplicitColumns]
	class deco
	{
		[Column] public long id { get; set; }
		[Column] public string title { get; set; }
		[Column] public bool draft { get; set; }
		[Column] public DateTime date_created { get; set; }
		[Column] public DateTime? date_edited { get; set; }
		[Column] public string content { get; set; }
	}
	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id")]
	[ExplicitColumns]
	class deco_explicit
	{
		[Column]
		public long id { get; set; }
		[Column]
		public string title { get; set; }
		[Column]
		public bool draft { get; set; }
		[Column]
		public DateTime date_created { get; set; }
		public string content { get; set; }
	}

	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id")]
	class deco_non_explicit
	{
		public long id { get; set; }
		public string title { get; set; }
		public bool draft { get; set; }
		public DateTime date_created { get; set; }
		[Ignore] public string content { get; set; }
	}
}
