using Microsoft.AspNetCore.Mvc;
using Stripe;
using BlissShop.Common.Configs;
using BlissShop.Abstraction;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;

namespace BlissShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly StripeConfig _stripeConfig;

    public PaymentController(StripeConfig stripeConfig, IPaymentService paymentService)
    {
        _stripeConfig = stripeConfig;
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CheckOut(PaymentRequest request)
    {
        var userId = HttpContext.GetUserId();
        var result = await _paymentService.Checkout(userId, request);
        return Ok(result);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook() 
    { 
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _stripeConfig.WebhookSecret);

        var result = await _paymentService.HandleWebhook(stripeEvent);

        return Ok(result);
    }
}