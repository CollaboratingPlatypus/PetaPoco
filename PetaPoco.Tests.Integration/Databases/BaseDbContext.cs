using System;

namespace PetaPoco.Tests.Integration
{
    public abstract class BaseDbContext : IDisposable
    {
        private BaseDbProviderFactory _provider;

        protected IDatabase DB { get; set; }
        protected string ProviderName { get; private set; }

        protected BaseDbContext(BaseDbProviderFactory provider)
        {
            _provider = provider;
            DB = _provider.Execute();
            ProviderName = _provider.ProviderName;
        }

        public void Dispose()
        {
            _provider?.Dispose();
            _provider = null;
            DB?.Dispose();
            DB = null;
        }
    }
}
