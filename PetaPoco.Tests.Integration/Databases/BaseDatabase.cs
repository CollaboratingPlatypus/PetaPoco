// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabase : IDisposable
    {
        private DBTestProvider _provider;
        protected IDatabase DB { get; set; }

        protected BaseDatabase(DBTestProvider provider)
        {
            _provider = provider;
            DB = _provider.Execute();
        }

        public void Dispose()
        {
            if (DB != null)
            {
                _provider.Dispose();
                _provider = null;
                DB.Dispose();
                DB = null;
            }
        }
    }
}