using Amazon.S3;
using Core.Data.EF;
using Core.Data.EF.MySql;
using Core.Data.EF.PostgreSql;
using Core.Data.EF.Repositories;
using Core.Data.EF.Sqlite;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.AttachmentServices;
using Core.Services.Post;
using Core.Services.Storage;
using Core.Services.Storage.Cloud;
using Core.Services.Storage.Local;
using Core.Services.Telegram;
using Core.Services.Youtube;
using Core.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Web.Caching.AWS;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Providers.AWS;
using System;
using Voxed.WebApp.Helpers;
using Voxed.WebApp.Services;
using Voxed.WebApp.Services.Moderation;

namespace Voxed.WebApp;

public static class DependencyContainer
{
    public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("RDS Connection: " + DatabaseHelpers.GetRDSConnectionString(configuration));

        var provider = configuration.GetValue<DatabaseProvider>("DatabaseProvider");
        services.AddDbContext<VoxedContext>(
            options => _ = provider switch
            {
                DatabaseProvider.Sqlite => options
                    .UseSqlite(configuration.GetConnectionString(nameof(DatabaseProvider.Sqlite)),
                        x => x.MigrationsAssembly(typeof(SqliteVoxedContext).Assembly.GetName().Name)),

                DatabaseProvider.MySql => options
                .UseMySql(
                    configuration.GetConnectionString(nameof(DatabaseProvider.MySql)),
                    ServerVersion.AutoDetect(configuration.GetConnectionString(nameof(DatabaseProvider.MySql))),
                    x => x.MigrationsAssembly(typeof(MySqlVoxedContext).Assembly.GetName().Name)),

                DatabaseProvider.PostgreSql => options
                .UseNpgsql(configuration.GetConnectionString(nameof(DatabaseProvider.PostgreSql)),
                    x => x.MigrationsAssembly(typeof(PostgreSqlVoxedContext).Assembly.GetName().Name)),

                _ => throw new Exception($"Unsupported database provider: {provider}")
            });
    }

    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IVoxedRepository, VoxedRepository>();
    }

    public static void RegisterLogger(this IServiceCollection services)
    {
        services.AddLogging(logger =>
        {
            logger.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);
            logger.SetMinimumLevel(LogLevel.Debug);
            logger.AddSimpleConsole();
        });
    }

    public static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultIdentity<User>(options =>
                options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<Role>()
            .AddEntityFrameworkStores<VoxedContext>()
            .AddErrorDescriber<SpanishIdentityErrorDescriber>();


        services
            .AddAuthentication()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]; ;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
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
            //options.User.AllowedUserNameCharacters =
            //"abcdefghijklmnopqrstuvwxyz0123456789";
            options.User.RequireUniqueEmail = false;
        });
    }

    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IPostService, PostService>();
        services.AddSingleton<IContentFormatterService, ContentFormatterService>();
        services.AddTransient<IUserVoxActionService, UserVoxActionService>();
        services.AddTransient<IModerationService, ModerationService>();
        services.AddSingleton<IYoutubeService, YoutubeService>();

        services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName));
        services.AddSingleton<ITelegramService, TelegramService>();

        services.Configure<AttachmentServiceOptions>(configuration.GetSection(AttachmentServiceOptions.SectionName));
        services.AddSingleton<IAttachmentService, AttachmentService>();

        if (true)
        {
            services
                .AddImageSharp()
                .Configure<AWSS3StorageImageProviderOptions>(options =>
                {
                    options.S3Buckets.Add(new AWSS3BucketClientOptions
                    {
                        BucketName = "post-attachments",
                        AccessKey = "AKIAT3LYSLSBEG32UEDZ",
                        AccessSecret = "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",
                        Region = "sa-east-1"
                    });
                })
                .ClearProviders()
                .AddProvider<AWSS3StorageImageProvider>()
                .Configure<AWSS3StorageCacheOptions>(options =>
                {
                    options.BucketName = "post-attachments/cache";
                    options.AccessKey = "AKIAT3LYSLSBEG32UEDZ";
                    options.AccessSecret = "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";
                    options.Region = "sa-east-1";

                    // Optionally create the cache bucket on startup if not already created.
                    AWSS3StorageCache.CreateIfNotExists(options, S3CannedACL.Private);
                })
                .SetCache<AWSS3StorageCache>();
        }
        else
        {
            services
                .AddImageSharp()
                .ClearProviders()
                .AddProvider<PhysicalFileSystemProvider>();
        }
    }

    public static void RegisterStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        if (false)
        {
            services.Configure<CloudStorageOptions>(configuration.GetSection(CloudStorageOptions.SectionName));
            services.AddSingleton<IStorage, CloudStorage>();
        }
        else
        {
            services.Configure<LocalStorageOptions>(configuration.GetSection(LocalStorageOptions.SectionName));
            services.AddSingleton<IStorage, LocalStorage>();
        }
    }


    public static void RegisterWebServices(this IServiceCollection services)
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
