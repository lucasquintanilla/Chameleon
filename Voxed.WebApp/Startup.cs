using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public void ConfigureServices(IServiceCollection services)
        {
            // New approach
            services.RegisterWebServices(_configuration);
            services.RegisterInfrastructureServices(_configuration);
            services.RegisterRepositories();
            services.RegisterServices(_configuration);
            services.RegisterIdentity();
        }

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    app.UseCors(myAllowSpecificOrigins);

        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Home/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }
        //    app.UseHttpsRedirection();
        //    //app.UseStaticFiles();
        //    var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
        //    app.UseStaticFiles(new StaticFileOptions
        //    {
        //        OnPrepareResponse = ctx =>
        //        {
        //            ctx.Context.Response.Headers.Append(
        //                 "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
        //        }
        //    });

        //    app.UseRouting();

        //    app.UseAuthentication();
        //    app.UseAuthorization();

        //    app.UseSession();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllerRoute(
        //            name: "default",
        //            pattern: "{controller=Home}/{action=Index}/{id?}");

        //        endpoints.MapRazorPages();
        //        //endpoints.MapBlazorHub();


        //        endpoints.MapHub<VoxedHub>("/hubs/notifications");
        //    });
        //}
    }
}
