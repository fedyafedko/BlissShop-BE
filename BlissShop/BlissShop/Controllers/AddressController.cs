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

    [HttpPost]
    public async Task<IActionResult> CreateAddress(CreateAddressDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.CreateAddressAsync(userId, dto);

        return Ok(result);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAddressesForUser()
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.GetAddressesAsync(userId);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAddress([FromQuery] Guid addressId)
    {
        var userId = HttpContext.GetUserId();
        var result = await _addressService.DeleteAddressAsync(userId, addressId);

        return result ? NoContent() : NotFound();
    }
}
