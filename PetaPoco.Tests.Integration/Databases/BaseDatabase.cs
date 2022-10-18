using System;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseDatabase : IDisposable
    {
        protected DBTestProvider DBTestProvider { get; set; }
        protected IDatabase DB { get; set; }
        protected string ProviderName { get; private set; }

        protected BaseDatabase(DBTestProvider dbTestProvider)
        {
            DBTestProvider = dbTestProvider;
            DB = dbTestProvider.Execute();
            ProviderName = dbTestProvider.ProviderName;
        }

        public void Dispose()
        {
            if (DB != null)
            {
                DBTestProvider.Dispose();
                DBTestProvider = null;
                DB.Dispose();
                DB = null;
            }
        }
    }
}
