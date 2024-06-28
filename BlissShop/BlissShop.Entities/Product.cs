using System.ComponentModel.DataAnnotations.Schema;


namespace BlissShop.Entities;

public class Product : EntityBase
{
    [ForeignKey(nameof(Shop))]
    public Guid ShopId { get; set; }
    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double TotalRating { get; set; } = 0;

    public Shop Shop { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public List<Order> Orders { get; set; } = null!;
    public List<ProductCartItem> ProductCartItem { get; set; } = null!;
    public List<Rating> Ratings { get; set; } = null!;
}
