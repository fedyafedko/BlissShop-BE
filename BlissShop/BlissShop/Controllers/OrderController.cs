using Microsoft.AspNetCore.Mvc;
using Stripe;
using BlissShop.Common.Configs;
using BlissShop.Abstraction;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using Microsoft.AspNetCore.Authorization;
using BlissShop.Common.DTO;

namespace BlissShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly StripeConfig _stripeConfig;

    public OrderController(
        IOrderService orderService,
        StripeConfig stripeConfig)
    {
        _orderService = orderService;
        _stripeConfig = stripeConfig;
    }

    /// <summary>
    /// CheckOut for payment.
    /// </summary>
    /// <param name="request"></param>
    /// <returns> This endpoint returns a status code.</returns>
    [HttpPost("[action]")]
    [Authorize]
    [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut(PaymentRequest request)
    {
        var userId = HttpContext.GetUserId();
        var result = await _orderService.Checkout(userId, request);
        return Ok(result);
    }

    /// <summary>
    /// Web hook for insert order.
    /// </summary>
    /// <returns> This endpoint returns a status code.</returns>
    [HttpPost("webhook")]
    [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Getting orders for user.
    /// </summary>
    /// <returns> This endpoint returns orders.</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _orderService.GetOrdersForUserAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Getting orders for seller.
    /// </summary>
    /// <returns> This endpoint returns orders.</returns>
    [HttpGet("[action]")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersForSeller()
    {
        var sellerId = HttpContext.GetUserId();
        var result = await _orderService.GetOrdersForSellerAsync(sellerId);
        return Ok(result);
    }

    /// <summary>
    /// Get order by id.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns> This endpoint returns an order.</returns>
    [HttpGet("[action]")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);
        return Ok(result);
    }

    /// <summary>
    /// Refund order.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns> This endpoint returns a result.</returns>
    [HttpPost("[action]")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refund(Guid orderId)
    {
        var userId = HttpContext.GetUserId();
        var result = await _orderService.Refund(userId, orderId);
        return result ? Ok() : BadRequest();  
    }

    /// <summary>
    /// Update order status.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns> This endpoint returns a result.</returns>
    [HttpPut("[action]")]
    [Authorize(Roles = "Seller")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApprovedOrder(Guid orderId)
    {
        var result = await _orderService.ApprovedOrderAsync(orderId);
        return result ? Ok() : BadRequest();
    }
}