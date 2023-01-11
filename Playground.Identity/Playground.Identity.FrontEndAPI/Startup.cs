using AutoFixture;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Base.Identity;
using Playground.Identity.DAL;
using System;

[assembly: WebJobsStartup(typeof(Playground.Identity.FrontEndAPI.Startup))]

namespace Playground.Identity.FrontEndAPI
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Nexus Identity
            builder.Services.AddWebJobs((configure) => { }).AddAccessTokenBinding();


            //Singleton in getting Connection String for Cosmos DB
            builder.Services.AddSingleton(s =>
            {
                var connectionString = Configuration.GetConnectionString("CosmosDB") ?? Configuration["CosmosDB"]; ;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid CosmosDB connection string in the appSettings.json file or your Azure Functions Settings.");
                }
                return new CosmosClientBuilder(connectionString).Build();
            });


            // For Unit of Work
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();

            // For Open API 
            builder.Services.AddSingleton<Fixture>(new Fixture())
                            .AddSingleton<IOpenApiConfigurationOptions>(_ =>
                            {
                                var options = new OpenApiConfigurationOptions()
                                {
                                    OpenApiVersion = OpenApiVersionType.V3
                                };

                                return options;
                            });
        }
    }
}
