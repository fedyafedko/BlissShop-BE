using BlissShop.Common.Requests;

namespace BlissShop.FluentEmail.MessageBase;

public class SupportMessage : EmailMessageBase
{
    public override string Subject => "Support Email";

    public override string TemplateName => nameof(SupportMessage);
    public SupportRequest Message { get; set; } = new SupportRequest();
}
