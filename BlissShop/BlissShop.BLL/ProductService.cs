using AutoMapper;
using BlissShop.Abstraction.Product;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests.ProductImage;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Shop> _shopRepository;
    private readonly FileConfig _fileConfig;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<Product> productRepository,
        IRepository<Shop> shopRepository,
        ILogger<ProductService> logger,
        IMapper mapper,
        FileConfig fileConfig,
        IWebHostEnvironment env)
    {
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _logger = logger;
        _mapper = mapper;
        _fileConfig = fileConfig;
        _env = env;
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

    public async Task<ProductImagesResponse> UploadImagesAsync(Guid userId, UploadProductImageRequest request)
    {
        var product = await _productRepository
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId)
            ?? throw new NotFoundException($"Product not found with such id: {request.ProductId}");

        if (product.Shop.SellerId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var contetntPath = _env.ContentRootPath;
        var path = Path.Combine(contetntPath, _fileConfig.FolderForProductImages, product.ShopId.ToString(), request.ProductId.ToString());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var result = new ProductImagesResponse();
        result.Paths ??= new List<string>();

        foreach (var image in request.Images)
        {
            var fileName = image.FileName;
            var ext = Path.GetExtension(fileName);

            if (!_fileConfig.FileExtensions.Contains(ext))
                throw new IncorrectParametersException("Invalid file extension");

            var filePath = Path.Combine(path, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            result.Paths.Add(string.Format(_fileConfig.Path, fileName));
        }

        return result;
    }

    public async Task<bool> DeleteImagesAsync(Guid userId, DeleteProductImageRequest request)
    {
        var product = await _productRepository
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId)
            ?? throw new NotFoundException($"Product not found with such id: {request.ProductId}");

        if (product.Shop.SellerId != userId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var wwwPath = _env.ContentRootPath;
        var productImegesPath = Path.Combine(wwwPath, _fileConfig.FolderForProductImages);
        foreach (var image in request.Images)
        {
            var path = Path.Combine(productImegesPath, product.ShopId.ToString(), request.ProductId.ToString(), image);

            if (!File.Exists(path))
                throw new NotFoundException("File not found");

            await Task.Run(() => File.Delete(path));
        }

        return true;
    }
}
