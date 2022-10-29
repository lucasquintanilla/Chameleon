using Core.Data.EF;
using Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp;

public class Program
{
    private const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();

        // move this to responsable class
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<VoxedContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            await new DataInitializer(context, userManager, roleManager).Initialize();
        }

        app.UseCors(myAllowSpecificOrigins);
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
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
        app.Run();
    }
}
