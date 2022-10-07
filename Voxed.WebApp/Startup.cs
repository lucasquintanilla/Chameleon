using Core.Data.EF;
using Core.Data.EF.MySql;
using Core.Data.EF.Repositories;
using Core.Data.EF.Sqlite;
using Core.Data.Repositories;
using Core.Entities;
using Core.Services.ImxtoService;
using Core.Services.Telegram;
using Core.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Services;

namespace Voxed.WebApp
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            #region Repositories

            services.AddTransient<IVoxedRepository, VoxedRepository>();

            #endregion

            //https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=dotnet-core-cli

            RegisterInfrastrutureServices(services);

            services.AddDefaultIdentity<User>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<VoxedContext>()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();

            RegisterServices(services);



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

        private void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IVoxService, VoxService>();
            services.AddSingleton<FormateadorService>();
            services.AddSingleton<FileUploadService>();
            services.AddSingleton<ImxtoService>();

            services.Configure<TelegramConfiguration>(_configuration.GetSection(TelegramConfiguration.SectionName));
            services.AddSingleton<TelegramService>();
            services.AddSingleton<YoutubeService>();
            //services.AddSingleton<GlobalMessageService>();
        }

        private void RegisterInfrastrutureServices(IServiceCollection services)
        {
            var provider = _configuration.GetValue("Provider", nameof(SqlProvider.MySql));
            services.AddDbContext<VoxedContext>(
                options => _ = provider switch
                {
                    nameof(SqlProvider.Sqlite) => options
                        .UseSqlite(
                            _configuration.GetConnectionString(nameof(SqlProvider.Sqlite)),
                            x => x.MigrationsAssembly(typeof(SqliteVoxedContext).Assembly.GetName().Name)),
                    //.UseLoggerFactory(ContextLoggerFactory),

                    nameof(SqlProvider.MySql) => options
                    .UseMySql(
                        _configuration.GetConnectionString(nameof(SqlProvider.MySql)),
                        ServerVersion.AutoDetect(_configuration.GetConnectionString(nameof(SqlProvider.MySql))),
                        x => x.MigrationsAssembly(typeof(MySqlVoxedContext).Assembly.GetName().Name)),
                    //.UseLoggerFactory(ContextLoggerFactory),

                    _ => throw new Exception($"Unsupported provider: {provider}")
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Elastic Apm Configuration must be before all configuration
            //app.UseAllElasticApm(Configuration);


            //app.UseCors(builder => builder
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    //.AllowCredentials()
            //    .AllowAnyHeader()
            //    .SetPreflightMaxAge(TimeSpan.FromDays(365)));

            app.UseCors(myAllowSpecificOrigins);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            //app.UseStaticFiles();
            var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append(
                         "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
                //endpoints.MapBlazorHub();


                endpoints.MapHub<VoxedHub>("/hubs/notifications");
            });
        }
    }

    public enum SqlProvider
    {
        MySql,
        Sqlite,
        SqlServer
    }
}
