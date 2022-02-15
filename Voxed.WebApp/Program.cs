using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
using Core.Data.EF;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;
using Core.Entities;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Extensions.Logging;
using Core.Data.EF.Sqlite;
using Core.Data.EF.MySql;
using Microsoft.Extensions.Logging;

namespace Voxed.WebApp
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Log.Logger.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();
                await CreateDbIfNotExists(host);
                host.Run();

                
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<VoxedContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<Role>>();

                    //await new DbInitializer(context, userManager, roleManager).Initialize();

                    //await SqliteInitialize(services);
                    await MySqlInitialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "localdb string connection: " + Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb"));
                    logger.LogError(ex, "An error occurred creating the DB.");
                    throw;
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)                
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, configuration) =>
                {
                    configuration.Enrich.FromLogContext()
                        //.Enrich.WithMachineName()
                        .WriteTo.Console()
                        .WriteTo.File("./logs/logs-.txt", rollingInterval: RollingInterval.Day)
                        //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://i-o-optimized-deployment-d7f8f8.es.eastus2.azure.elastic-cloud.com:9243"))
                        //{
                        //    IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.Now:yyyy-MM}",
                        //    AutoRegisterTemplate = true,
                        //    NumberOfShards = 2,
                        //    NumberOfReplicas = 1,
                        //    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "pZbpxWQhkWGsBmVAGuXh7nTj"),
                        //    MinimumLogEventLevel = LogEventLevel.Information
                        //})
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .ReadFrom.Configuration(context.Configuration);

                })
                ;

        private static async Task SqliteInitialize(IServiceProvider services)
        {
            var context = services.GetRequiredService<SqliteVoxedContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            await new DbInitializer(context, userManager, roleManager).Initialize();
        }

        private static async Task MySqlInitialize(IServiceProvider services)
        {
            var context = services.GetRequiredService<MySqlVoxedContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            await new DbInitializer(context, userManager, roleManager).Initialize();
        }
    }
}
