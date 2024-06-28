using BlissShop.Common.Configs;
using BlissShop.Hangfire.Abstractions;
using BlissShop.Hangfire.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BlissShop.Hangfire.Extensions;

public static class HangfireExtensions
{
    public static void SetupHangfire(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var hangfireService = services.GetRequiredService<IHangfireService>();
        var options = services.GetRequiredService<HangfireConfig>();

        hangfireService.SetupRecurring<CalculateTotalRating>(
            CalculateTotalRating.Id,
            options.CalculateTotalRatingCron);
    }
}
