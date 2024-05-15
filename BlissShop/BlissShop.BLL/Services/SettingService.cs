using BlissShop.Abstraction;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.Requests;
using BlissShop.FluentEmail.MessageBase;

namespace BlissShop.BLL.Services;
public class SettingService : ISettingService
{
    private readonly IEmailService _emailService;

    public SettingService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task<bool> SendEmailSupport(SupportRequest request)
    {
        var emailMessage = new SupportMessage { Message = request };

        var result = _emailService.SendAsync(emailMessage, request.Email);
        
        return result;
    }
}
