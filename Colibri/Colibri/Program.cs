using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Colibri.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Colibri
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // WebHost: pass Arguments from the command Line
            var host = BuildWebHost(args);

            //RunSeeding(host);

            host.Run();
        }

        // Helper Method to seed the Data before the WebHost will run
        private static void RunSeeding(IWebHost host)
        {
            // scoped service
            var scopedFactory = host.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopedFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<ColibriSeeder>();
                seeder.SeedAsync().Wait();
            }
        }

        // WebHost: configure
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(SetupConfiguration)
                .UseStartup<Startup>()
                .Build();

        private static void SetupConfiguration(WebHostBuilderContext ctx, IConfigurationBuilder builder)
        {
            // remove the default configuration options
            builder.Sources.Clear();

            // configuration file as JSON
            // file optional: false
            // reload on change: true
            // HIERARCHY: BOTTOM -> UP!
            builder.AddJsonFile("config.json", false, true)
                .AddXmlFile("config.xml", true)
                .AddEnvironmentVariables();
        }
    }
}
