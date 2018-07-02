// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

#if NETCOREAPP
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PetaPoco.Tests.Integration
{
    public class AppSetting
    {
        public List<ConnectionStringSetting> ConnectionStrings { get; set;  } = new List<ConnectionStringSetting>();

        public static AppSetting Load()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
                
            var app = new AppSetting();
            config.GetSection("App").Bind(app);
            return app;
        }

        public class ConnectionStringSetting
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }
    }
}
#endif