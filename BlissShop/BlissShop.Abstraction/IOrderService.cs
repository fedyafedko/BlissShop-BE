using BlissShop.Common.DTO;
using BlissShop.Common.Requests;
using Stripe;

namespace BlissShop.Abstraction;

public interface IOrderService
{
    Task<string> Checkout(Guid userId, PaymentRequest request);
    Task<bool> HandleWebhook(Event stripeEvent);
    Task<List<OrderDTO>> GetOrdersForUserAsync(Guid userId);
    Task<OrderDTO> GetOrderAsync(Guid orderId);
    Task<Refund> Refund(Guid userId, Guid orderId)
}
