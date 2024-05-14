using BlissShop.Entities;

namespace BlissShop.FluentEmail.MessageBase;

public class AprovedShopMessage : EmailMessageBase
{
    public override string Subject => "Aproved Shop";
    public override string TemplateName => nameof(AprovedShopMessage);
    public Shop Shop { get; set; } = null!;
    public string Uri { get; set; } = string.Empty;
}