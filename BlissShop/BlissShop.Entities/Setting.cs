using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class Setting : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public bool IsDarkMode { get; set; } = false;
    public bool IsEmailNotification { get; set; } = true;
    public User User { get; set; } = null!;
}