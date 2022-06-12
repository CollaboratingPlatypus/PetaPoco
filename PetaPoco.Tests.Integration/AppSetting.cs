using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace PetaPoco.Tests.Integration
{
    public class AppSetting
    {
        public List<ConnectionStringSetting> ConnectionStrings { get; } = new List<ConnectionStringSetting>();

        private static AppSetting _instance;

        public static AppSetting Instance => _instance;

        static AppSetting()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
                
            var app = new AppSetting();
            config.GetSection("App").Bind(app);
            _instance = app;
        }

        public ConnectionStringSetting ConnectionStringFor(string name)
        {
            return ConnectionStrings.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public class ConnectionStringSetting
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }
    }
}