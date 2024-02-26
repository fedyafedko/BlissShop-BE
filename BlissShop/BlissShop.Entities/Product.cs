using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Product : EntityBase
{
    [ForeignKey(nameof(Shop))]
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public Shop Shop { get; set; } = null!;
    public List<Order> Orders { get; set; } = null!;
    public List<ProductCartItem> ProductCartItem { get; set; } = null!;
}
