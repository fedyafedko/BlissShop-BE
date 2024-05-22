namespace BlissShop.Common.Configs;

public class CallbackUrisConfig : ConfigBase
{
    public string ResetPasswordUri { get; set; } = string.Empty;
    public string AprovedShopUri { get; set; } = string.Empty;
    public string NewProductUri { get; set; } = string.Empty;
}
