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
	}


	// Attributed not-so-true poco
	[TableNameAttribute("petapoco")]
	[PrimaryKeyAttribute("id")]
	[ExplicitColumnsAttribute]
	class deco
	{
		[ColumnAttribute] public long id { get; set; }
		[ColumnAttribute] public string title { get; set; }
		[ColumnAttribute] public bool draft { get; set; }
		[ColumnAttribute] public DateTime date_created { get; set; }
		[ColumnAttribute] public DateTime? date_edited { get; set; }
		[ColumnAttribute] public string content { get; set; }
		[ColumnAttribute] public State state { get; set; }
	}
	// Attributed not-so-true poco
	[TableNameAttribute("petapoco")]
	[PrimaryKeyAttribute("id")]
	[ExplicitColumnsAttribute]
	class deco_explicit
	{
		[ColumnAttribute] public long id { get; set; }
		[ColumnAttribute] public string title { get; set; }
		[ColumnAttribute] public bool draft { get; set; }
		[ColumnAttribute] public DateTime date_created { get; set; }
		[ColumnAttribute] public State state { get; set; }
		public string content { get; set; }
	}

	// Attributed not-so-true poco
	[TableNameAttribute("petapoco")]
	[PrimaryKeyAttribute("id")]
	class deco_non_explicit
	{
		public long id { get; set; }
		public string title { get; set; }
		public bool draft { get; set; }
		public DateTime date_created { get; set; }
		public State state { get; set; }
		[IgnoreAttribute] public string content { get; set; }
	}
}
