namespace BlissShop.Common.Configs;

public class StripeConfig : ConfigBase
{
    public string SecretKey { get; set; } = string.Empty;
    public string PublishKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string PaymentMethodTypes { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}
