// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using System;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabaseTest : IDisposable
    {
        protected Database DB { get; set; }

        private DBTestProvider _provider;

        protected BaseDatabaseTest(DBTestProvider provider)
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