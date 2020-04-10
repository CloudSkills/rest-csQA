using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csQA;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]
namespace csQA
{
    class Startup : FunctionsStartup
    {
        private static IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<Constants>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Settings").Bind(settings);
                });

            builder.Services.AddSingleton((s) => {
                string endpoint = configuration["COSMOS_DB_DATABASE_URL"];
                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json file or your Azure Functions Settings.");
                }

                string authKey = configuration["COSMOS_DB_DATABASE_KEY"];
                if (string.IsNullOrEmpty(authKey) || string.Equals(authKey, "Super secret key"))
                {
                    throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json file or your Azure Functions Settings.");
                }

                CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(endpoint, authKey);
                return configurationBuilder
                    .Build();
            });


        }
    }
}
