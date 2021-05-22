using Core.Data.EF;
using Core.Data.EF.MySql;
using Core.Data.EF.Repositories;
using Core.Data.EF.Sqlite;
using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Cors Configuration must be before MVC / Razor Configuration
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
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

            #region Repositories

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IVoxRepository, VoxRepository>();
            services.AddTransient<IVoxedRepository, VoxedRepository>();

            #endregion

            services.AddDbContext<SqliteVoxedContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString(nameof(SqlProvider.Sqlite))));

            services.AddDbContext<MySqlVoxedContext>(options =>
                   options.UseMySql(Configuration.GetConnectionString(nameof(SqlProvider.MySql)), 
                   ServerVersion.AutoDetect(Configuration.GetConnectionString(nameof(SqlProvider.MySql)))));

            var provider = Configuration.GetValue("Provider", nameof(SqlProvider.Sqlite));
            services.AddDbContext<VoxedContext>(
                            options => _ = provider switch
                            {
                                nameof(SqlProvider.Sqlite) => options.UseSqlite(
                                    Configuration.GetConnectionString(nameof(SqlProvider.Sqlite)),
                                    x => x.MigrationsAssembly(typeof(SqliteVoxedContext).Assembly.GetName().Name)),

                                nameof(SqlProvider.MySql) => options.UseMySql(
                                    Configuration.GetConnectionString(nameof(SqlProvider.MySql)),
                                    ServerVersion.AutoDetect(Configuration.GetConnectionString(nameof(SqlProvider.MySql))),
                                    x => x.MigrationsAssembly(typeof(MySqlVoxedContext).Assembly.GetName().Name)),

                                //nameof(SqlProvider.SqlServer) => options.UseSqlServer(
                                //Configuration.GetConnectionString(nameof(SqlProvider.SqlServer)),
                                //x => x.MigrationsAssembly("SqlServerMigrations")),

                                _ => throw new Exception($"Unsupported provider: {provider}")
                            });

            services.AddDefaultIdentity<User>(options => 
                    options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Role>()
                .AddEntityFrameworkStores<VoxedContext>()
                .AddErrorDescriber<SpanishIdentityErrorDescriber>();

            services.AddSingleton<FormateadorService>();
            services.AddSingleton<FileUploadService>();

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
                options.Cookie.HttpOnly = true;
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

            //services.Configure<FileUploadServiceConfiguration>(Configuration.GetSection("FileUploadService"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Elastic Apm Configuration must be before all configuration
            app.UseAllElasticApm(Configuration);


            //app.UseCors(builder => builder
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    //.AllowCredentials()
            //    .AllowAnyHeader()
            //    .SetPreflightMaxAge(TimeSpan.FromDays(365)));

            app.UseCors(MyAllowSpecificOrigins);

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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

                endpoints.MapHub<VoxedHub>("/hubs/notifications");
            });
        }
    }

    public enum SqlProvider{
        MySql,
        Sqlite,
        SqlServer
    }
}
