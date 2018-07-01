// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

#if NETCOREAPP
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
#endif

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : DBTestProvider
    {
#if NETCOREAPP
        protected override IDatabase Database
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var config = builder.Build();
                
                var appConfig = new AppSetting();
                config.GetSection("App").Bind(appConfig);


                return DatabaseConfiguration.Build()
                    .UsingConnectionString(appConfig.ConnectionStrings.First(c => c.Name == "postgres").ConnectionString)
                    .UsingProviderName(appConfig.ConnectionStrings.First(c => c.Name == "postgres").ProviderName)
                    .Create();
            }
        }
#else
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("postgres").Create();
#endif

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}