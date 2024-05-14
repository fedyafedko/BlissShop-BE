using BlissShop.Entities;

namespace BlissShop.FluentEmail.MessageBase;

public class ShopIsNotAprovedMessage : EmailMessageBase
{
    public override string Subject => "Aproved Shop";
    public override string TemplateName => nameof(ShopIsNotAprovedMessage);
    public Shop Shop { get; set; } = null!;
}