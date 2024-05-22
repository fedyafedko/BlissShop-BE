using BlissShop.Common.DTO.Products;

namespace BlissShop.FluentEmail.MessageBase;

public class NewProductMessage : EmailMessageBase
{
    public override string Subject => "New product";

    public override string TemplateName => nameof(NewProductMessage);

    public ProductDTO Product { get; set; } = null!;
    public string Uri { get; set; } = string.Empty;
}
