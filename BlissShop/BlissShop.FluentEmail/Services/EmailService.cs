using BlissShop.FluentEmail.MessageBase;
using BlissShop.Abstraction.FluentEmail;
using FluentEmail.Core;
using BlissShop.Common.Configs;

namespace BlissShop.FluentEmail.Services;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly EmailConfig _emailConfig;

    public EmailService(IFluentEmail fluentEmail, EmailConfig emailConfig)
    {
        _fluentEmail = fluentEmail;
        _emailConfig = emailConfig;
    }

    public async Task<bool> SendAsync<T>(T message, string? from = null)
        where T : EmailMessageBase
    {
        var path = $@"{Directory.GetCurrentDirectory()}{_emailConfig.MessagePath}\{message.TemplateName}.cshtml";
        var sendEmail = await _fluentEmail
                  .SetFrom(from ?? _emailConfig.DefaultFromEmail)
                  .To(message.Recipient ?? _emailConfig.DefaultFromEmail)
                  .Subject(message.Subject)
                  .UsingTemplateFromFile(path, message)
                  .SendAsync();

        return sendEmail.Successful;
    }
}
