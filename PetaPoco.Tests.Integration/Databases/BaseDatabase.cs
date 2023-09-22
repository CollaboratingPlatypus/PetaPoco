using System;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabase : IDisposable
    {
        private DBTestProvider _provider;
        protected IDatabase DB { get; set; }
        protected string ProviderName { get; private set; }

        protected BaseDatabase(DBTestProvider provider)
        {
            _provider = provider;
            DB = _provider.Execute();
            ProviderName = _provider.ProviderName;
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
