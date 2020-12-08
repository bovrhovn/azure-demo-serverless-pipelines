using System;
using Demo.Interfaces;
using Demo.Services;
using Demo.Web.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StorageOptions>(Configuration.GetSection("StorageOptions"));

            var storageSettings = Configuration.GetSection("StorageOptions").Get<StorageOptions>();
            services.AddScoped<IStorageWorker, AzureStorageWorker>(_ =>
                new AzureStorageWorker(storageSettings.ConnectionString, storageSettings.Container));

            services.AddApplicationInsightsTelemetry();

            services.AddRazorPages().AddRazorPagesOptions(options =>
                options.Conventions.AddPageRoute("/Info/Index", ""));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}