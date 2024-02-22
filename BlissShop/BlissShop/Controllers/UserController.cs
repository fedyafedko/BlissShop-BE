using BlissShop.Abstraction.Users;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> UploadAvatar(Guid userId, IFormFile avatar)
    {
        var result = await _userService.UploadAvatarAsync(userId, avatar);
        return Ok(result);
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteAvatar(string avatar)
    {
        var result = await _userService.DeleteAvatarAsync(avatar);
        return result ? NoContent() : NotFound();
    }
}
