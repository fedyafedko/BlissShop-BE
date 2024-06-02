namespace BlissShop.Common.DTO.Settings;

public class SettingDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsDarkMode { get; set; }
    public bool IsEmailNotification { get; set; }
}