using BlissShop.FluentEmail.MessageBase;

namespace BlissShop.Abstraction.FluentEmail;

public interface IEmailService
{
    Task<bool> SendAsync<T>(string to, T message)
        where T : EmailMessageBase;
}
