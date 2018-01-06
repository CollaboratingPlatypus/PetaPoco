namespace PetaPoco.Tests.Integration.Databases
{
#if !NETFULL
    using System;

    using Core;

    using Internal;
    
    using Microsoft.Extensions.Configuration;

    partial class DBTestProvider
    {
        protected IDatabaseBuildConfiguration CreateConfiguration(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ArgumentException("Argument is null or empty", "connectionStringName");
            }

            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = cfg.GetSection($"ConnectionStrings:{connectionStringName}:ConnectionString").Value;
            var providerName = cfg.GetSection($"ConnectionStrings:{connectionStringName}:ProviderName").Value;
            var provider = GetProvider(providerName, connectionString);

            return DatabaseConfiguration.Build()
                .UsingConnectionStringName(connectionStringName)
                .UsingConnectionString(connectionString)
                .UsingProvider(provider);
        }

        private IProvider GetProvider(string typeName, string connectionString)
        {
            if (typeName.StartsWith("Oracle"))
                return Singleton<OracleClientDatabaseProvider>.Instance;
            if (typeName.StartsWith("Microsoft.Data.Sqlite"))
                return Singleton<MsSQLiteDatabaseProvider>.Instance;

            return DatabaseProvider.Resolve(typeName, false, connectionString);
        }
    }
#endif
}
