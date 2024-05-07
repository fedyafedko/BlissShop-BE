using BlissShop.Abstraction;
using BlissShop.Common.Configs;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace BlissShop.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<ProductCartItem> _productCartItemRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly StripeConfig _stripeConfig;

    public PaymentService(
        IRepository<ProductCart> productCartRepository,
        StripeConfig stripeConfig,
        UserManager<User> userManager,
        IRepository<Order> orderRepository,
        IRepository<ProductCartItem> productCartItemRepository)
    {
        _productCartRepository = productCartRepository;
        _stripeConfig = stripeConfig;
        _userManager = userManager;
        _orderRepository = orderRepository;
        _productCartItemRepository = productCartItemRepository;
    }

    public async Task<string> Checkout(Guid cartId, Guid userId)
    {
        var productCart = await _productCartRepository
            .Include(x => x.ProductCartItems).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == cartId && x.UserId == userId)
            ?? throw new NotFoundException("Product not found");

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        List<SessionLineItemOptions> items = new();

        productCart.ProductCartItems.ForEach(item =>
        {
            items.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)item.Product.Price * 100,
                    Currency = _stripeConfig.Currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Id.ToString(),
                    },
                },
                Quantity = item.Quantity,
            });
        });

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                _stripeConfig.PaymentMethodTypes
            },  
            Metadata = new Dictionary<string, string>
            {
                { "cartId", cartId.ToString() }
            },
            CustomerEmail = user.Email,
            LineItems = items,
            SuccessUrl = _stripeConfig.SuccessUrl,
            CancelUrl = _stripeConfig.CancelUrl,
            Mode = _stripeConfig.Mode
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public async Task<bool> HandleWebhook(Event stripeEvent)
    {
        var session = stripeEvent.Type == Events.CheckoutSessionCompleted
            ? stripeEvent.Data.Object as Session
            : null;

        if (session == null)
            throw new NotFoundException("Session not found");

        var user = await _userManager.FindByEmailAsync(session.CustomerEmail)
            ?? throw new NotFoundException("User not found");

        var cartId = new Guid(session.Metadata["cartId"]);

        var productCart = await _productCartRepository
            .Include(x => x.ProductCartItems)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == cartId && x.UserId == user.Id)
            ?? throw new NotFoundException("Product not found");

        var order = new List<Order>();
        
        foreach (var item in productCart.ProductCartItems)
        {
            order.Add(new Order
            {
                BuyerId = user.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                IsPaid = true
            });
        }

        var result = await _orderRepository.InsertManyAsync(order);

        if (result)
            await _productCartItemRepository.DeleteManyAsync(productCart.ProductCartItems);

        return result;
    }
}
