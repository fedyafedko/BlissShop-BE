using System.ComponentModel.DataAnnotations.Schema;

namespace BlissShop.Entities;

public class ProductCartItem : EntityBase
{
    [ForeignKey(nameof(ProductCart))]
    public Guid ProductCartId { get; set; }
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public ProductCart ProductCart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
