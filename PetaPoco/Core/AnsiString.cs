// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace PetaPoco
{
	/// <summary>
	/// Wrap strings in an instance of this class to force use of DBType.AnsiString
	/// </summary>
	public class AnsiString
	{
		/// <summary>
		/// Constructs an AnsiString
		/// </summary>
		/// <param name="str">The C# string to be converted to ANSI before being passed to the DB</param>
		public AnsiString(string str)
		{
			Value = str;
		}

		/// <summary>
		/// The string value
		/// </summary>
		public string Value 
		{ 
			get; 
			private set; 
		}
	}

}
