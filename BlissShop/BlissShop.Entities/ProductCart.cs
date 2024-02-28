using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class ProductCart : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid UserId { get; set; }
    public double TotalPrice { get; set; }

    public User User { get; set; } = null!;
    public List<ProductCartItem> ProductCartItems { get; set; } = null!;
}