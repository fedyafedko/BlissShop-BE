using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class ShopFollower : EntityBase
{
    [ForeignKey(nameof(Shop))]
    public Guid ShopId { get; set; }
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }

    public Shop Shop { get; set; } = null!;
    public User User { get; set; } = null!;
}