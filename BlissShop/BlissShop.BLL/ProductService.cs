using AutoMapper;
using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Google.Apis.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Shop> _shopRepository;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<Product> productRepository,
        IRepository<Shop> shopRepository,
        ILogger<ProductService> logger,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ProductDTO> AddProductAsync(Guid sellerId, CreateProductDTO dto)
    {
        var shop = await _shopRepository.FirstOrDefaultAsync(x => x.Id == dto.ShopId)
            ?? throw new NotFoundException($"Shop not found with such id: {dto.ShopId}");

        if (shop.SellerId != sellerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var product = _mapper.Map<Product>(dto);
        product.ShopId = shop.Id;

        var result = await _productRepository.InsertAsync(product);

        if (!result)
            _logger.LogError("Product was not created");

        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<bool> DeleteProductAsync(Guid sellerId, Guid id)
    {
        var product = await _productRepository
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Product not found with such id: {id}");

        if(product.Shop.SellerId != sellerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var result = await _productRepository.DeleteAsync(product);

        if(!result)
            _logger.LogError("Product was not deleted");

        return result;
    }

    public async Task<ProductDTO> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Product not found with such id: {id}");

        return _mapper.Map<ProductDTO>(product);
    }

    public async Task<List<ProductDTO>> GetProductsForShopAsync(Guid shopId)
    {
        var query = _productRepository.Where(x => x.ShopId == shopId);
        var products = await query.ToListAsync();

        return _mapper.Map<List<ProductDTO>>(products);
    }

    public async Task<ProductDTO> UpdateProductAsync(Guid sellerId, Guid id, UpdateProductDTO dto)
    {
        var product = await _productRepository
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Product not found with such id: {id}");

        if (product.Shop.SellerId != sellerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        _mapper.Map(dto, product);

        var result = await _productRepository.UpdateAsync(product);

        if (!result)
            _logger.LogError("Product was not updated");

        return _mapper.Map<ProductDTO>(product);
    }
}
