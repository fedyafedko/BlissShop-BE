using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Products;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Requests;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.Entities.Enums;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace BlissShop.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<ProductCartItem> _productCartItemRepository;
    private readonly IRepository<ProductCart> _productCartRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<User> _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly StripeConfig _stripeConfig;
    private readonly IMapper _mapper;

    public OrderService(
        IRepository<ProductCartItem> productCartItemRepository,
        IRepository<ProductCart> productCartRepository,
        IRepository<Order> orderRepository,
        IRepository<User> userRepository,
        UserManager<User> userManager,
        IEmailService emailService,
        StripeConfig stripeConfig,
        IMapper mapper)
    {
        _productCartItemRepository = productCartItemRepository;
        _productCartRepository = productCartRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _emailService = emailService;
        _stripeConfig = stripeConfig;
        _mapper = mapper;
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

    public async Task<OrderDTO> GetOrderAsync(Guid orderId)
    {
        var order = await _orderRepository
            .Include(x => x.Product)
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new NotFoundException("Order not found");

        return _mapper.Map<OrderDTO>(order);
    }

    public async Task<bool> ApprovedOrderAsync(Guid orderId)
    {
        var order = await _orderRepository
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new NotFoundException("Order not found");

        if (order.Status != StatusOrder.Pending)
            throw new RestrictedAccessException("Order has already been approved");

        order.Status = StatusOrder.Processing;

        return await _orderRepository.UpdateAsync(order);
    }

    public async Task<List<OrderDTO>> GetOrdersForUserAsync(Guid userId)
    {
        var orders = await _orderRepository
            .Include(x => x.Product)
            .Include(x => x.Address)
            .Where(x => x.BuyerId == userId)
            .Select(x => x)
            .ToListAsync();

        return _mapper.Map<List<OrderDTO>>(orders);
    }

    public async Task<List<OrderDTO>> GetOrdersForSellerAsync(Guid sellerId)
    {
        var orders = await _orderRepository
            .Include(x => x.Product)
            .ThenInclude(x => x.Shop)
            .Include(x => x.Address)
            .Where(x => x.Product.Shop.SellerId == sellerId)
            .Select(x => x)
            .ToListAsync();

        return _mapper.Map<List<OrderDTO>>(orders);
    }

    public async Task<bool> HandleWebhook(Event stripeEvent)
    {
        var session = stripeEvent.Type == Events.CheckoutSessionCompleted
            ? stripeEvent.Data.Object as Session
            : null;

        if (session == null)
            throw new NotFoundException("Session not found");

        var paymentIntentId = session.PaymentIntentId;

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

        var charge = paymentIntent.LatestChargeId;

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
                IsPaid = true,
                CreateAt = DateTime.Now,
                ChargeId = charge
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

    public async Task<bool> Refund(Guid userId, Guid orderId)
    {
        var order = await _orderRepository
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new NotFoundException("Order not found");

        if (userId != order.BuyerId)
            throw new RestrictedAccessException("You are not the owner and do not have permission to perform this action.");

        var options = new RefundCreateOptions { Charge = order.ChargeId };
        var service = new RefundService();
        var result = await service.CreateAsync(options);

        if (result.Status == "succeeded")
        {
            order.Status = StatusOrder.Refund;
            await _orderRepository.UpdateAsync(order);

            return true;
        }
            
        return false;
    }
}
