namespace BlissShop.Common.Configs;

public class EmailConfig : ConfigBase
{
    public string MessagePath { get; set; } = string.Empty;
    public string DefaultFromEmail { get; set; } = string.Empty;
}
