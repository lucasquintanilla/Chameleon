using Core.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voxed.WebApp.Services;

namespace Voxed.WebApp;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        LogConfigurationValues(_configuration);
        services.RegisterWebServices();
        services.RegisterLogger();
        services.RegisterInfrastructureServices(_configuration);
        services.RegisterRepositories();
        services.RegisterServices(_configuration);
        services.RegisterStorageImageProvider(_configuration);
        services.RegisterStorageServices(_configuration);
        services.RegisterIdentity(_configuration);
        services.AddTransient<INotificationService, NotificationService>();
    }

    static void LogConfigurationValues(IConfiguration configuration)
    {
        foreach (var section in configuration.GetChildren())
        {
            LogSectionValues(section);
        }
    }

    static void LogSectionValues(IConfigurationSection section, string parentKey = "")
    {
        foreach (var keyValuePair in section.AsEnumerable())
        {
            var fullKey = string.IsNullOrEmpty(parentKey) ? keyValuePair.Key : $"{parentKey}:{keyValuePair.Key}";
            var value = keyValuePair.Value;

            System.Console.WriteLine($"{fullKey}: {value}");
        }

        foreach (var subsection in section.GetChildren())
        {
            var fullKey = string.IsNullOrEmpty(parentKey) ? subsection.Key : $"{parentKey}:{subsection.Key}";
            LogSectionValues(subsection, fullKey);
        }
    }
}
