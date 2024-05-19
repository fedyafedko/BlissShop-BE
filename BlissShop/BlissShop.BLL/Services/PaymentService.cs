using BlissShop.Abstraction;
using BlissShop.Common.Configs;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests;
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
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<ProductCartItem> _productCartItemRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly StripeConfig _stripeConfig;

    public PaymentService(
        IRepository<ProductCart> productCartRepository,
        StripeConfig stripeConfig,
        UserManager<User> userManager,
        IRepository<Order> orderRepository,
        IRepository<ProductCartItem> productCartItemRepository,
        IRepository<User> userRepository)
    {
        _productCartRepository = productCartRepository;
        _stripeConfig = stripeConfig;
        _userManager = userManager;
        _orderRepository = orderRepository;
        _productCartItemRepository = productCartItemRepository;
        _userRepository = userRepository;
    }

    public async Task<string> Checkout(Guid userId, PaymentRequest request)
    {
        var productCart = await _productCartRepository
            .Include(x => x.ProductCartItems).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == request.CartId && x.UserId == userId)
            ?? throw new NotFoundException("Product not found");

        var user = await _userRepository
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new NotFoundException("User not found");

        if (user.Addresses.All(x => x.Id != request.AddressId))
            throw new NotFoundException("Address not found");

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
                { "cartId", request.CartId.ToString() },
                { "addressId", request.AddressId.ToString() }
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
        var addressId = new Guid(session.Metadata["addressId"]);

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
                AddressId = addressId,
                Quantity = item.Quantity,
                IsPaid = true
            });
        }

        var result = await _orderRepository.InsertManyAsync(order);

        if (result)
        {
            productCart.TotalPrice = 0;
            await _productCartItemRepository.DeleteManyAsync(productCart.ProductCartItems);
        }

        return result;
    }
}
