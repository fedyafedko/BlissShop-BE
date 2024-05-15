using BlissShop.Common.Requests;

namespace BlissShop.Abstraction;

public interface ISettingService
{
    Task<bool> SendEmailSupport(SupportRequest request);
}
