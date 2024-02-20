namespace BlissShop.FluentEmail.MessageBase;

public class ConfirmedEmailMessage : EmailMessageBase
{
    public override string Subject => "ConfirmedEmail";
    public override string TemplateName => nameof(ConfirmedEmailMessage);
    public int Code { get; set; }
}
