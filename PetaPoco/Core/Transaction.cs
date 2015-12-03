﻿// PetaPoco - A Tiny ORMish thing for your POCO's.
// Copyright © 2011-2012 Topten Software.  All Rights Reserved.
 
using System;

namespace PetaPoco
{
	public interface ITransaction : IDisposable
	{
		void Complete();
	}

	/// <summary>
	/// Transaction object helps maintain transaction depth counts
	/// </summary>
	public class Transaction : ITransaction
	{
		public Transaction(Database db)
		{
			_db = db;
			_db.BeginTransaction();
		}

		public void Complete()
		{
			_db.CompleteTransaction();
			_db = null;
		}

		public void Dispose()
		{
			if (_db != null)
				_db.AbortTransaction();
		}

		Database _db;
	}
}
