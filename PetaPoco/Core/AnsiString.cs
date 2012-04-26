// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace PetaPoco
{
	/// <summary>
	/// Wrap strings in an instance of this class to for use of DBType.AnsiString
	/// </summary>
	public class AnsiString
	{
		public AnsiString(string str)
		{
			Value = str;
		}

		public string Value 
		{ 
			get; 
			private set; 
		}
	}

}
