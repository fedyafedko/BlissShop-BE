using BlissShop.Abstraction.Product;
using BlissShop.Common.DTO.Products;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class ProductCartService : IProductCartService
{
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<ProductCartItem> _productCartItemsRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ProductCartService> _logger;

    public ProductCartService(
        IRepository<ProductCart> productCartRepository,
        IRepository<ProductCartItem> productCartItemsRepository,
        IRepository<Product> productRepository,
        UserManager<User> userManager,
        ILogger<ProductCartService> logger)
    {
        _productCartRepository = productCartRepository;
        _productCartItemsRepository = productCartItemsRepository;
        _productRepository = productRepository;
        _userManager = userManager;
        _logger = logger;
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
            ?? throw new Exception("Product not found");

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

        var productItem = _productCartItemsRepository.Include(x => x.Product).FirstOrDefault(x => x.ProductId == productId && x.ProductCartId == productCart.Id)
            ?? throw new Exception("Product not found in cart");

        var result = await _productCartItemsRepository.DeleteAsync(productItem);

        if(!result) 
        {
            _logger.LogError("Failed to remove product from cart");
        }

        productCart.TotalPrice -= (double)productItem.Product.Price * productItem.Quantity;
        await _productCartRepository.UpdateAsync(productCart);

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
}
