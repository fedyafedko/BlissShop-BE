using BlissShop.Abstraction.Users;
using BlissShop.Common.DTO.User;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.UploadAvatarAsync(userId, avatar);
        return Ok(result);
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteAvatar(string avatar)
    {
        var result = await _userService.DeleteAvatarAsync(avatar);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.Me(userId);
        return Ok(result);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.ChangePasswordAsync(userId, request);
        return result ? Ok() : BadRequest();
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> EditProfile(UpdateUserDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.EditProfile(userId, dto);
        return Ok(result);
    }
}
