using BlissShop.Common.DTO.Products;
using BlissShop.Entities;

namespace BlissShop.FluentEmail.MessageBase;

public class OrderMessage : EmailMessageBase
{
    public override string Subject => "Order Confirmation";

    public override string TemplateName => nameof(OrderMessage);

    public List<ProductDTO> Products { get; set; } = null!;

    public double TotalPrice { get; set; }
}
