﻿using BlissShop.Common.DTO.Products;

namespace BlissShop.Abstraction.Product;

public interface IProductService
{
    Task<ProductDTO> AddProductAsync(Guid sellerId, CreateProductDTO dto);
    Task<ProductDTO> UpdateProductAsync(Guid sellerId, Guid id, UpdateProductDTO dto);
    Task<bool> DeleteProductAsync(Guid sellerId, Guid id);
    Task<ProductDTO> GetProductByIdAsync(Guid id);
    Task<List<ProductDTO>> GetProductsForShopAsync(Guid shopId);
}
