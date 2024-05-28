using Microsoft.AspNetCore.Mvc;
using Stripe;
using BlissShop.Common.Configs;
using BlissShop.Abstraction;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;

namespace BlissShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly StripeConfig _stripeConfig;

    public OrderController(StripeConfig stripeConfig, IOrderService orderService)
    {
        _stripeConfig = stripeConfig;
        _orderService = orderService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CheckOut(PaymentRequest request)
    {
        var userId = HttpContext.GetUserId();
        var result = await _orderService.Checkout(userId, request);
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

        var result = await _orderService.HandleWebhook(stripeEvent);

        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _orderService.GetOrdersForUserAsync(userId);
        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);
        return Ok(result);
    }
}