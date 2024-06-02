using BlissShop.Common.DTO.Settings;
using BlissShop.Common.Requests;

namespace BlissShop.Abstraction;

public interface ISettingService
{
    Task<SettingDTO> GetSettingsForUserAsync(Guid userId);
    Task<bool> SendEmailSupport(SupportRequest request);
    Task<bool> UpdateSettingsAsync(Guid userId, UpdateSettingDTO dto);
}
