using AutoMapper;
using BlissShop.Abstraction.Product;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extensions;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ProductCartService : IProductCartService
{
    private readonly IRepository<ProductCartItem> _productCartItemsRepository;
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly ProductImagesConfig _productImagesConfig;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProductCartService> _logger;
    private readonly IMapper _mapper;

    public ProductCartService(
        IRepository<ProductCartItem> productCartItemsRepository,
        IRepository<ProductCart> productCartRepository,
        IRepository<Product> productRepository,
        ProductImagesConfig productImagesConfig,
        UserManager<User> userManager,
        IWebHostEnvironment env,
        ILogger<ProductCartService> logger,
        IMapper mapper)
    {
        _productCartItemsRepository = productCartItemsRepository;
        _productCartRepository = productCartRepository;
        _productRepository = productRepository;
        _productImagesConfig = productImagesConfig;
        _userManager = userManager;
        _env = env;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> AddToProductCart(Guid userId, AddProductCartDTO dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new Exception("User not found");

        var productCart = await _productCartRepository
            .Include(x => x.ProductCartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if(productCart == null)
        {
            productCart = new ProductCart
            {
                UserId = userId,
                TotalPrice = 0
            };

            await _productCartRepository.InsertAsync(productCart);
        }

        var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == dto.ProductId)
            ?? throw new NotFoundException("Product not found");

        if (product.Quantity < dto.Quantity)
        {
            throw new NotFoundException("Not enough products in stock");
        }

        if (productCart.ProductCartItems != null &&productCart.ProductCartItems.Any(x => x.ProductId == dto.ProductId))
        {
            await UpdateExistingProductCartItem(productCart, dto, product);

            return true;
        }

        var result = await AddNewProductCartItem(productCart, dto, product);

        if(!result)
        {
            _logger.LogError("Failed to add product to cart");
        }

        return result;
    }

    public async Task<bool> RemoveFromProductCart(Guid userId, Guid productId)
    {
        var productCart = _productCartRepository.FirstOrDefault(x => x.UserId == userId)
            ?? throw new Exception("Product cart not found");

        var productItem = _productCartItemsRepository
            .Include(x => x.Product)
            .FirstOrDefault(x => x.ProductId == productId && x.ProductCartId == productCart.Id)
            ?? throw new Exception("Product not found in cart");

        bool result = false;

        foreach (var item in productCart.ProductCartItems.ToList())
        {
            if (item.ProductId == productId)
            {
                item.Quantity -= 1;
                productCart.TotalPrice -= (double)productItem.Product.Price;
                if (item.Quantity == 0)
                {
                    await _productCartItemsRepository.DeleteAsync(item);
                    productCart.TotalPrice = 0;
                    result = await _productCartRepository.UpdateAsync(productCart);
                }
                else
                {
                    result = await _productCartItemsRepository.UpdateAsync(item);
                }
            }
        }

        if(!result) 
        {
            _logger.LogError("Failed to remove product from cart");
        }

        return result;
    }

    private async Task UpdateExistingProductCartItem(ProductCart productCart, AddProductCartDTO dto, Product product)
    {
        var productItem = productCart.ProductCartItems.FirstOrDefault(x => x.ProductId == dto.ProductId);
        if (productItem == null)
        {
            throw new Exception("Product not found in cart");
        }

        productItem.Quantity += dto.Quantity;
        await _productCartItemsRepository.UpdateAsync(productItem);
        productCart.TotalPrice += (double)product.Price * dto.Quantity;
        await _productCartRepository.UpdateAsync(productCart);
    }

    private async Task<bool> AddNewProductCartItem(ProductCart productCart, AddProductCartDTO dto, Product product)
    {
        var productCartItem = new ProductCartItem
        {
            ProductId = dto.ProductId,
            ProductCartId = productCart.Id,
            Quantity = dto.Quantity,
        };

        var result = await _productCartItemsRepository.InsertAsync(productCartItem);

        if (!result)
        {
            _logger.LogError("Failed to add product to cart");
        }

        productCart.TotalPrice += (double)product.Price * dto.Quantity;
        await _productCartRepository.UpdateAsync(productCart);
        return result;
    }

    public async Task<ProductCartResponse> GetProductCart(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new Exception("User not found");

        var productCart = await _productCartRepository
            .Include(x => x.ProductCartItems)
            .ThenInclude(x => x.Product)
            .ThenInclude(x => x.Shop)
            .FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new Exception("Product cart not found");

        var products = new List<ProductCartItemResponse>();

        foreach (var item in productCart.ProductCartItems)
        {
            products.Add(new ProductCartItemResponse
            {
                Product = _mapper.Map<ProductDTO>(item.Product),
                Quantity = item.Quantity,
            });
        }

        var result = new ProductCartResponse
        {
            Products = products,
            TotalPrice = (decimal)productCart.TotalPrice,
        };

        foreach (var product in result.Products)
        {
            product.Product.ImagesPath = _env.ContentRootPath.GetImagePath(product.Product, _productImagesConfig);
        }

        return result;
    }
}
