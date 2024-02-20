using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class UserRegister : EntityBase
{
    public int Code { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsCodeRegenerated { get; set; }

    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
