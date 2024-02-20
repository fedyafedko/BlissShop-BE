namespace BlissShop.Common.Configs;

public class AuthConfig : ConfigBase
{
    public int ConfirmationCodeLenght { get; set; }
    public TimeSpan ConfirmationCodeLifetime { get; set; }
}
