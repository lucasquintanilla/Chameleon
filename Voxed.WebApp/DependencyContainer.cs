using Core.Data.EF;
using Core.Data.EF.MySql;
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

namespace Voxed.WebApp;

public static class DependencyContainer
{
    public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli
        var provider = configuration.GetValue("Provider", nameof(SqlProvider.MySql));
        services.AddDbContext<VoxedContext>(
            options => _ = provider switch
            {
                nameof(SqlProvider.Sqlite) => options
                    .UseSqlite(
                        configuration.GetConnectionString(nameof(SqlProvider.Sqlite)),
                        x => x.MigrationsAssembly(typeof(SqliteVoxedContext).Assembly.GetName().Name)),
                //.UseLoggerFactory(ContextLoggerFactory),

                nameof(SqlProvider.MySql) => options
                .UseMySql(
                    configuration.GetConnectionString(nameof(SqlProvider.MySql)),
                    ServerVersion.AutoDetect(configuration.GetConnectionString(nameof(SqlProvider.MySql))),
                    x => x.MigrationsAssembly(typeof(MySqlVoxedContext).Assembly.GetName().Name)),
                //.UseLoggerFactory(ContextLoggerFactory),

                _ => throw new Exception($"Unsupported provider: {provider}")
            });
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
        services.AddSingleton<IContentFormatterService, ContentFormatterService>();
        services.AddSingleton<IAttachmentService, AttachmentService>();
        services.AddTransient<IUserVoxActionService, UserVoxActionService>();
        services.AddTransient<IContentReportService, ContentReportService>();

        services.Configure<TelegramConfiguration>(configuration.GetSection(TelegramConfiguration.SectionName));
        services.Configure<AttachmentServiceConfiguration>(configuration.GetSection(AttachmentServiceConfiguration.SectionName));
        
        services.AddSingleton<TelegramService>();
        services.AddSingleton<YoutubeService>();
        //services.AddSingleton<GlobalMessageService>();
    }

    public static void RegisterWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // Cors Configuration must be before MVC / Razor Configuration
        services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  builder =>
                                  {
                                  //builder.WithOrigins("http://localhost",
                                  //                    "http://www.contoso.com");

                                      builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddDistributedMemoryCache();
        services.AddSession();
        services.AddScoped<TraceIPAttribute>();

        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            //options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        //Por default necesita que estes autenticado para entrar a los controllers 
        //services.AddAuthorization(options =>
        //{
        //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        //        .RequireAuthenticatedUser()
        //        .Build();
        //});

        services.AddSignalR();
    }
}

public enum SqlProvider
{
    MySql,
    Sqlite,
    SqlServer
}
