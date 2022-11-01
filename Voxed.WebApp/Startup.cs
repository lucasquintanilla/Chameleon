using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.RegisterWebServices();
        services.RegisterInfrastructureServices(_configuration);
        services.RegisterRepositories();
        services.RegisterServices(_configuration);
        services.RegisterIdentity();
    }
}
