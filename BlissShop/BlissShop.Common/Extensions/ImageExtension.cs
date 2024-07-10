using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Products;

namespace BlissShop.Common.Extensions;

public static class ImageExtension
{
    public static List<string> GetImagePath(this string rootPath, ProductDTO product, ProductImagesConfig config)
    {
        var filePath = Path.Combine(
                rootPath,
                config.Folder,
                product.Shop.Id.ToString(),
                product.Id.ToString());

        if (!Directory.Exists(filePath))
            return new List<string>();

        var files = Directory.GetFiles(filePath).Select(x => Path.GetFileName(x));

        var images = new List<string>();

        foreach (var file in files)
        {
            images.Add(string.Format(config.Path, product.Shop.Id, product.Id, file));
        }

        return images;
    }
}
