using Core.Data.EF;
using Core.Data.EF.Repositories;
using Core.Data.EF.Sqlite;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services;
using Core.Services.AttachmentServices;
using Core.Services.Telegram;
using Core.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Voxed.WebApp.Services;


namespace Voxed.WebApp
{
    public static class DependencyContainer
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli
            //var mysqlConnectionString = configuration["MYSQLCONNSTR_localdb"];
            //var mysqlconnectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb") ?? configuration.GetConnectionString(nameof(SqlProvider.MySql));
            var provider = configuration.GetValue("Provider", nameof(SqlProvider.MySql));
            services.AddDbContext<VoxedContext>(
                options => _ = provider switch
                {
                    nameof(SqlProvider.Sqlite) => options
                        .UseSqlite(
                            configuration.GetConnectionString(nameof(SqlProvider.Sqlite)),
                            x => x.MigrationsAssembly(typeof(SqliteVoxedContext).Assembly.GetName().Name)),
                    //.UseLoggerFactory(ContextLoggerFactory),

                    //nameof(SqlProvider.MySql) => options
                    //.UseMySql(
                    //    mysqlConnectionString,
                    //    ServerVersion.AutoDetect(mysqlConnectionString),
                    //    x => x.MigrationsAssembly(typeof(MySqlVoxedContext).Assembly.GetName().Name)),
                    ////.UseLoggerFactory(ContextLoggerFactory),

                    _ => throw new Exception($"Unsupported provider: {provider}")
                });

            //services.AddDbContext<VoxedContext>(x=>x.UseMySql(ServerVersion.AutoDetect(mysqlConnectionString)));
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddTransient<IVoxedRepository, VoxedRepository>();
        }

        public static void RegisterIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<VoxedContext>()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();


            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = "921895924714-26f3feivedthc93e7jnnvaf2nkf497hk.apps.googleusercontent.com";
                googleOptions.ClientSecret = "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";
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyz0123456789";
                options.User.RequireUniqueEmail = false;
            });
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IVoxService, VoxService>();
            services.AddSingleton<FormateadorService>();
            services.AddSingleton<IAttachmentService, AttachmentService>();
            services.AddTransient<IUserVoxActionService, UserVoxActionService>();
            services.AddTransient<IContentReportService, ContentReportService>();

            services.Configure<TelegramConfiguration>(configuration.GetSection(TelegramConfiguration.SectionName));
            services.Configure<AttachmentServiceConfiguration>(configuration.GetSection(AttachmentServiceConfiguration.SectionName));
            //services.AddOptions<FileUploadServiceConfiguration>()
            //    .Bind(configuration.GetSection(FileUploadServiceConfiguration.SectionName))
            //    .Configure<IConfiguration>((options, config) =>
            //    {
            //        services
            //        options.WebRootPath =
            //    }); 
            services.AddSingleton<TelegramService>();
            services.AddSingleton<YoutubeService>();
            //services.AddSingleton<GlobalMessageService>();
        }
    }

    public enum SqlProvider
    {
        MySql,
        Sqlite,
        SqlServer
    }
}
