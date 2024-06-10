using BlissShop.Abstraction;
using BlissShop.Common.DTO.Address;
using BlissShop.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    /// <summary>
    /// Adding address for user.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns> This endpoint returns an address.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAddress(CreateAddressDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.CreateAddressAsync(userId, dto);

        return Ok(result);
    }

    /// <summary>
    /// Getting addresses for user.
    /// </summary>
    /// <returns> This endpoint returns an addresses for user.</returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(List<AddressDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddressesForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.GetAddressesAsync(userId);

        return Ok(result);
    }

    /// <summary>
    /// Getting addresses for user.
    /// </summary>
    /// <param name="addressId"></param>
    /// <returns> This endpoint returns status code.</returns>
    [HttpDelete]
    [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress([FromQuery] Guid addressId)
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.DeleteAddressAsync(userId, addressId);

        return result ? NoContent() : NotFound();
    }
}
