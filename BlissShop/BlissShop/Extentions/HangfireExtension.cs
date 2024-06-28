using BlissShop.Hangfire.Abstractions;
using BlissShop.Hangfire.Services;
using Hangfire;
using Hangfire.SqlServer;

namespace BlissShop.Extentions;

public static class HangfireExtension
{
    public static void AddHangfire(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddHangfire(
            cfg => cfg.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        JobStorage.Current = new SqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

        services.AddHangfireServer();
        services.AddScoped<IHangfireService, HangfireService>();
    }
}
