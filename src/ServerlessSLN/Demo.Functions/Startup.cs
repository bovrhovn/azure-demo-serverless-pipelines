using System;
using Demo.Functions;
using Demo.Functions.Interfaces;
using Demo.Functions.Services;
using Demo.Interfaces;
using Demo.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Demo.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var storageKey = Environment.GetEnvironmentVariable("StorageKey");

            builder.Services.AddScoped<IStorageWorker, AzureStorageWorker>(_ =>
                new AzureStorageWorker(storageKey,
                    Environment.GetEnvironmentVariable("ContainerName")));
            builder.Services.AddScoped<IDOOperations, DOOperationService>(_ =>
                new DOOperationService(Environment.GetEnvironmentVariable("Pat")));
        }
    }
}