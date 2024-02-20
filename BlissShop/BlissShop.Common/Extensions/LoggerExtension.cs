using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.Common.Extentions;

public static class LoggerExtension
{
    public static void LogIdentityErrors<T>(this ILogger<T> logger, User user, IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("\n", result.Errors.Select(e => e.Description));
        logger.LogError("User with id {Id} has errors:\n{Errors}", user.Id, errors);
    }
}
