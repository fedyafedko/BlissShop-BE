using BlissShop.Common.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlissShop.Common.Utility;

public class ConfigService
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ConfigService(
        IServiceCollection services,
        IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public ConfigService AddConfig<T>()
            where T : ConfigBase, new()
    {
        T config = new T();

        var section = _configuration.GetSection(typeof(T).Name);
        section.Bind(config);

        _services.AddSingleton(config);

        return this;
    }
}