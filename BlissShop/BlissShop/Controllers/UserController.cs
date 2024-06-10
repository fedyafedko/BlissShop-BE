using BlissShop.Abstraction.Users;
using BlissShop.Common.DTO.User;
using BlissShop.Common.Extensions;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
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

    /// <summary>
    /// Upload an avatar for user.
    /// </summary>
    /// <param name="avatar"></param>
    /// <returns> This endpoint returns an avatar path.</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(AvatarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.UploadAvatarAsync(userId, avatar);
        return Ok(result);
    }

    /// <summary>
    /// Delete an avatar for user.
    /// </summary>
    /// <param name="avatar"></param>
    /// <returns> This endpoint returns a status code.</returns>
    [HttpDelete("[action]")]
    [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAvatar(string avatar)
    {
        var result = await _userService.DeleteAvatarAsync(avatar);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Get user by id.
    /// </summary>
    /// <returns> This endpoint returns an user.</returns>
    [HttpGet("[action]")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me()
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.Me(userId);
        return Ok(result);
    }

    /// <summary>
    /// Change password for user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns> This endpoint returns a status code.</returns>
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.ChangePasswordAsync(userId, request);
        return result ? Ok() : BadRequest();
    }

    /// <summary>
    /// Edit profile for user.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns> This endpoint returns an user.</returns>
    [HttpPut("[action]")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditProfile(UpdateUserDTO dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await _userService.EditProfile(userId, dto);
        return Ok(result);
    }
}
