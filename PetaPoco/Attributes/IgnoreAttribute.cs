// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace PetaPoco
{
	/// <summary>
	/// Use the Ignore attribute on POCO class properties that shouldn't be mapped
	/// by PetaPoco.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreAttribute : Attribute
	{
	}

}
