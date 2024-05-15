using System.Net;
using System.Net.Mail;

namespace BlissShop.Common.Extentions;

public static class FluentEmailExtension
{
    public static void FluentEmail(this IServiceCollection services, ConfigurationManager configuration)
    {
        var emailSettings = configuration.GetSection("EmailConfig");
        SmtpClient client = new SmtpClient
        {
            Credentials = new NetworkCredential(emailSettings["DefaultFromEmail"], emailSettings["Password"]),
            Host = emailSettings["Host"]!,
            Port = 587,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        var defaultFromEmail = emailSettings["DefaultFromEmail"];
        services.AddFluentEmail(defaultFromEmail)
            .AddSmtpSender(client)
            .AddRazorRenderer();
    }
}
