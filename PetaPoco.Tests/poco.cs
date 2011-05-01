using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace PetaPoco.Tests
{

	enum State
	{
		Yes,
		No,
		Maybe,
	}

	// Non-decorated true poco
	class poco
	{
		public long id { get; set; }
		public string title { get; set; }
		public bool draft { get; set; }
		public DateTime date_created { get; set; }
		public DateTime? date_edited { get; set; }
		public string content { get; set; }
		public State state { get; set; }
		[Column("col w space")] public int col_w_space { get; set; }
		public float? nullreal { get; set; }
	}


	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id", sequenceName="article_id_seq")]
	[ExplicitColumns]
	class deco
	{
		[Column] public long id { get; set; }
		[Column] public string title { get; set; }
		[Column] public bool draft { get; set; }
		[Column] public DateTime date_created { get; set; }
		[Column] public DateTime? date_edited { get; set; }
		[Column] public string content { get; set; }
		[Column] public State state { get; set; }
		[Column("col w space")] public int col_w_space { get; set; }
		[Column] public float? nullreal { get; set; }
	}
	// Attributed not-so-true poco
	[TableName("petapoco")]
	[PrimaryKey("id")]
	[ExplicitColumns]
	class deco_explicit
	{
		[Column]public long id { get; set; }
		[Column]public string title { get; set; }
		[Column]public bool draft { get; set; }
		[Column]public DateTime date_created { get; set; }
		[Column]public State state { get; set; }
		public string content { get; set; }
		[Column("col w space")]public int col_w_space { get; set; }
		[Column] public float? nullreal { get; set; }
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
		public State state { get; set; }
		[Ignore] public string content { get; set; }
		[Column("col w space")] public int col_w_space { get; set; }
		public float? nullreal { get; set; }
	}

	[TableName("petapoco2")]
	[PrimaryKey("email", autoIncrement=false)]
	class petapoco2
	{
		public string email { get; set; }
		public string name { get; set; }
	}
}
