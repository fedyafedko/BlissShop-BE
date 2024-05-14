using BlissShop.FluentEmail.MessageBase;

namespace BlissShop.Abstraction.FluentEmail;

public interface IEmailService
{
    Task<bool> SendAsync<T>(T message)
        where T : EmailMessageBase;
}
