namespace BlissShop.Common.Configs;

public class FileConfig : ConfigBase
{
    public string FolderForUserAvatar { get; set; } = string.Empty;
    public string FolderForShopAvatar { get; set; } = string.Empty;
    public string FolderForProductImages { get; set; } = string.Empty;
    public List<string> FileExtensions { get; set; } = null!;
    public string Path { get; set; } = string.Empty;
}
