using AutoMapper;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Abstraction.Product;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using BlissShop.Common.Requests.ProductImage;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ProductService : IProductService
{
    private readonly IRepository<ShopFollower> _followerRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Shop> _shopRepository;
    private readonly CallbackUrisConfig _callbackUrisConfig;
    private readonly ProductImagesConfig _productImagesConfig;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IRepository<ShopFollower> followerRepository,
        IRepository<Product> productRepository,
        IRepository<Shop> shopRepository,
        IRepository<Category> categoryRepository,
        CallbackUrisConfig callbackUrisConfig,
        ProductImagesConfig productImagesConfig,
        IEmailService emailService,
        IWebHostEnvironment env,
        ILogger<ProductService> logger,
        IMapper mapper)
    {
        _followerRepository = followerRepository;
        _productRepository = productRepository;
        _shopRepository = shopRepository;
        _categoryRepository = categoryRepository;
        _callbackUrisConfig = callbackUrisConfig;
        _productImagesConfig = productImagesConfig;
        _emailService = emailService;
        _env = env;
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

        var productMessage = _mapper.Map<ProductDTO>(product);
        productMessage.ImagesPath = _env.ContentRootPath.GetImagePath(productMessage, _productImagesConfig);

        await SendMessagesForFollowersAsync(shop.Id, productMessage);

        return productMessage;
    }

    public async Task<bool> DeleteProductAsync(Guid sellerId, Guid id)
    {
        var product = await _productRepository
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Product not found with such id: {id}");

        if (product.Shop.SellerId != sellerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var result = await _productRepository.DeleteAsync(product);

        if (!result)
            _logger.LogError("Product was not deleted");

        return result;
    }

    public async Task<ProductDTO> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository
            .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Product not found with such id: {id}");

        var result = _mapper.Map<ProductDTO>(product);

        result.ImagesPath = _env.ContentRootPath.GetImagePath(result, _productImagesConfig);

        return result;
    }

    public async Task<List<ProductDTO>> GetProductsForShopAsync(Guid shopId)
    {
        var query = _productRepository.Where(x => x.ShopId == shopId);
        var products = await query.ToListAsync();

        var result = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in result)
        {
            product.ImagesPath = _env.ContentRootPath.GetImagePath(product, _productImagesConfig);
        }

        return result;
    }

    public async Task<List<ProductDTO>> GetProductForCategoryAsync(Guid categoryId)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == categoryId)
            ?? throw new NotFoundException($"Category not found with such id: {categoryId}");

        var products = await _productRepository
            .Include(x => x.Shop)
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync();

        var result = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in result)
        {
            product.ImagesPath = _env.ContentRootPath.GetImagePath(product, _productImagesConfig);
        }

        return result;
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
        var path = Path.Combine(contetntPath, _productImagesConfig.Folder, product.ShopId.ToString(), request.ProductId.ToString());

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var result = new ProductImagesResponse();
        result.Paths ??= new List<string>();

        foreach (var image in request.Images)
        {
            var fileName = image.FileName;
            var ext = Path.GetExtension(fileName);

            if (!_productImagesConfig.FileExtensions.Contains(ext))
                throw new IncorrectParametersException("Invalid file extension");

            var uniqueSuffix = DateTime.UtcNow.Ticks;
            fileName = $"product_{uniqueSuffix}{ext}";
            var filePath = Path.Combine(path, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            result.Paths.Add(string.Format(_productImagesConfig.Path, product.ShopId, product.Id, fileName));
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
        var productImegesPath = Path.Combine(wwwPath, _productImagesConfig.Folder);
        foreach (var image in request.Images)
        {
            var path = Path.Combine(productImegesPath, product.ShopId.ToString(), request.ProductId.ToString(), image);

            if (!File.Exists(path))
                throw new NotFoundException("File not found");

            File.Delete(path);
        }

        return true;
    }
    
    private async Task<bool> SendMessagesForFollowersAsync(Guid shopId, ProductDTO product)
    {
        var followers = await _followerRepository
            .Include(x => x.User)
            .ThenInclude(x => x.Setting)
            .Where(x => x.ShopId == shopId)
            .ToListAsync();

        //ToDo: Send messages to followers with settings
        var emails = followers
            .Where(x => x.User.Setting.IsEmailNotification)
            .Select(x => x.User.Email)
            .ToList();

        var messages = new List<NewProductMessage>();

        if(emails.Count == 0)
            return true;

        foreach (var email in emails)
        {
            var message = new NewProductMessage
            {
                Recipient = email!,
                Product = product,
                Uri = string.Format(_callbackUrisConfig.NewProductUri, product.Id)
            };
            messages.Add(message);
        }

        var result = await _emailService.SendManyAsync(messages);

        return result;
    }

    public async Task<PageList<ProductDTO>> SearchProductAsync(SearchProductRequest request)
    {
        var products = await _productRepository.ToListAsync();

        if (!string.IsNullOrEmpty(request.Search))
        {
            products = products
                .Where(x => x.Name.Contains(request.Search) || x.Tags.Contains(request.Search))
                .ToList();
        }

        var searchProducts = _mapper.Map<List<ProductDTO>>(products);

        foreach (var product in searchProducts)
        {
            product.ImagesPath = _env.ContentRootPath.GetImagePath(product, _productImagesConfig);
        }

        var result = searchProducts.Pagination(request.Page, request.PageSize);

        return result;
    }
}
