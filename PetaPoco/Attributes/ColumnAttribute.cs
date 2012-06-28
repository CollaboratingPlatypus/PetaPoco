// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace PetaPoco
{
	/// <summary>
	/// For explicit poco properties, marks the property as a column and optionally 
	/// supplies the DB column name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : Attribute
	{
		public ColumnAttribute() 
		{
			ForceToUtc = false;
		}
		
		public ColumnAttribute(string Name) 
		{ 
			this.Name = Name;
			ForceToUtc = false;
		}

		public string Name 
		{ 
			get; 
			set; 
		}

		public bool ForceToUtc
		{
			get;
			set;
		}
	}

}
