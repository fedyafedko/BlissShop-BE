using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Order : EntityBase
{
    [ForeignKey(nameof(User))]
    public Guid BuyerId { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
