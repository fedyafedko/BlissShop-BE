using BlissShop.Abstraction.Product;
using BlissShop.Hangfire.Abstractions;

namespace BlissShop.Hangfire.Jobs;

public class CalculateTotalRating : IJob
{
    private readonly IProductService _productService;

    public CalculateTotalRating(IProductService productService)
    {
        _productService = productService;
    }

    public static string Id => nameof(CalculateTotalRating);

    public Task Run(CancellationToken cancellationToken = default) =>
        _productService.AddTotalRatingForProductAsync();
}
