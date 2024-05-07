using Stripe;

namespace BlissShop.Abstraction;

public interface IPaymentService
{
    Task<string> Checkout(Guid productId, Guid userId);
    Task<bool> HandleWebhook(Event stripeEvent);
}
