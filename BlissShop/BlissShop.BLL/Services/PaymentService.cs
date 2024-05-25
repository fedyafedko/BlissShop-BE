using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace BlissShop.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<ProductCartItem> _productCartItemRepository;
    private readonly ProductImagesConfig _productImagesConfig;
    private readonly IRepository<Order> _orderRepository;
    private readonly IWebHostEnvironment _env;
    private readonly StripeConfig _stripeConfig;
    private readonly IMapper _mapper;

    public PaymentService(
        IRepository<ProductCart> productCartRepository,
        StripeConfig stripeConfig,
        UserManager<User> userManager,
        IRepository<Order> orderRepository,
        IRepository<ProductCartItem> productCartItemRepository,
        IRepository<User> userRepository,
        IEmailService emailService,
        IMapper mapper,
        IWebHostEnvironment env,
        ProductImagesConfig productImagesConfig)
    {
        _productCartRepository = productCartRepository;
        _stripeConfig = stripeConfig;
        _userManager = userManager;
        _orderRepository = orderRepository;
        _productCartItemRepository = productCartItemRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _mapper = mapper;
        _env = env;
        _productImagesConfig = productImagesConfig;
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
                        Name = item.Product.Name,
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
            await SendOrderMessageAsync(user, productCart);
            productCart.TotalPrice = 0;
            await _productCartItemRepository.DeleteManyAsync(productCart.ProductCartItems);
        }

        return result;
    }

    private async Task<bool> SendOrderMessageAsync(User user, ProductCart productCart)
    {
        var products = productCart.ProductCartItems.Select(x => _mapper.Map<ProductDTO>(x.Product)).ToList();

        var result = await _emailService.SendAsync(new OrderMessage
        {
            Recipient = user.Email!,
            Products = products,
            TotalPrice = productCart.TotalPrice
        });

        return result;
    }
}
