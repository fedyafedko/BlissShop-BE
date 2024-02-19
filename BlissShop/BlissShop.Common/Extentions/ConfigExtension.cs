using BlissShop.Common.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlissShop.Common.Extentions;

public static class ConfigExtension
{
    public static IServiceCollection ConfigsAssembly(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ConfigService> configService)
    {
        var builder = new ConfigService(services, configuration);
        configService(builder);

        return services;
    }
}
