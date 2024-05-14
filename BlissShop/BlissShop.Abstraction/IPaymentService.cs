using BlissShop.Common.Requests;
using Stripe;

namespace BlissShop.Abstraction;

public interface IPaymentService
{
    Task<string> Checkout(Guid userId, PaymentRequest request);
    Task<bool> HandleWebhook(Event stripeEvent);
}
