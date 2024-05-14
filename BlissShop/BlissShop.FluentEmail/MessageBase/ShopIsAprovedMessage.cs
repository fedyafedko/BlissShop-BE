using BlissShop.Entities;

namespace BlissShop.FluentEmail.MessageBase;

public class ShopIsAprovedMessage : EmailMessageBase
{
    public override string Subject => "Aproved Shop";
    public override string TemplateName => nameof(ShopIsAprovedMessage);
    public Shop Shop { get; set; } = null!;
}
