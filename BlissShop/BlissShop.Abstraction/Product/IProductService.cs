using BlissShop.Common.DTO.Products;
using BlissShop.Common.Requests;
using BlissShop.Common.Requests.ProductImage;
using BlissShop.Common.Responses;
using BlissShop.Entities.Enums;

namespace BlissShop.Abstraction.Product;

public interface IProductService
{
    Task<ProductDTO> AddProductAsync(Guid sellerId, CreateProductDTO dto);
    Task<ProductDTO> UpdateProductAsync(Guid sellerId, Guid id, UpdateProductDTO dto);
    Task<bool> DeleteProductAsync(Guid sellerId, Guid id);
    Task<ProductDTO> GetProductByIdAsync(Guid id);
    Task<PageList<ProductDTO>> GetProductsForShopAsync(Guid shopId, SearchProductRequest reques, Sorting? sorting);
    Task<ProductImagesResponse> UploadImagesAsync(Guid userId, UploadProductImageRequest request);
    Task<bool> DeleteImagesAsync(Guid userId, DeleteProductImageRequest request);
    Task<PageList<ProductDTO>> SearchProductAsync(SearchProductRequest request, Sorting? sorting);
    Task<PageList<ProductDTO>> GetProductForCategoryAsync(Guid categoryId, SearchProductRequest request, Sorting? sorting);
    Task AddTotalRatingForProductAsync();
}
