using Core.Data.EF;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Voxed.WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Log.Logger.Information("ConnectionString: " + Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb"));
                var host = CreateHostBuilder(args).Build();
                await CreateDbIfNotExists(host);
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                Log.Logger.Error(ex.StackTrace);
                throw;
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
                    await new DataInitializer(context, userManager, roleManager).Initialize();
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
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddAzureWebAppDiagnostics();
                })
                .ConfigureServices(services =>
                {
                    services.Configure<AzureFileLoggerOptions>(options =>
                    {
                        options.FileName = "first-azure-log";
                        options.FileSizeLimit = 50 * 1024;
                        options.RetainedFileCountLimit = 10;
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        //.ConfigureLogging(logging =>
        //        //{
        //        //    logging.ClearProviders();
        //        //    // We have to be precise on the logging levels
        //        //    logging.AddConsole();
        //        //    logging.AddDebug();
        //        //    logging.AddAzureWebAppDiagnostics();
        //        //})
        //        //.ConfigureServices(services =>
        //        //{
        //        //    services.Configure<AzureFileLoggerOptions>(options =>
        //        //    {
        //        //        options.FileName = "my-azure-diagnostics-";
        //        //        options.FileSizeLimit = 50 * 1024;
        //        //        options.RetainedFileCountLimit = 5;
        //        //    });
        //        //})
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        })
        //        .UseSerilog((context, configuration) =>
        //        {
        //            configuration.Enrich.FromLogContext()
        //                //.Enrich.WithMachineName()
        //                .WriteTo.Console()
        //                .WriteTo.File("./logs/logs-.txt", rollingInterval: RollingInterval.Day)
        //                //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://i-o-optimized-deployment-d7f8f8.es.eastus2.azure.elastic-cloud.com:9243"))
        //                //{
        //                //    IndexFormat = $"{context.Configuration["ApplicationName"]}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.Now:yyyy-MM}",
        //                //    AutoRegisterTemplate = true,
        //                //    NumberOfShards = 2,
        //                //    NumberOfReplicas = 1,
        //                //    ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "pZbpxWQhkWGsBmVAGuXh7nTj"),
        //                //    MinimumLogEventLevel = LogEventLevel.Information
        //                //})
        //                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        //                .ReadFrom.Configuration(context.Configuration);
        //        })
        //        ;

        //private static async Task SqliteInitialize(IServiceProvider services)
        //{
        //    var context = services.GetRequiredService<VoxedContext>();
        //    var userManager = services.GetRequiredService<UserManager<User>>();
        //    var roleManager = services.GetRequiredService<RoleManager<Role>>();

        //    await new DbInitializer(context, userManager, roleManager).Initialize();
        //}

        //private static async Task MySqlInitialize(IServiceProvider services)
        //{
        //    var context = services.GetRequiredService<VoxedContext>();
        //    var userManager = services.GetRequiredService<UserManager<User>>();
        //    var roleManager = services.GetRequiredService<RoleManager<Role>>();

        //    await new DbInitializer(context, userManager, roleManager).Initialize();
        //}
    }
}
