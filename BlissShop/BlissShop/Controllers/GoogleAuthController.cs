using BlissShop.Abstraction.Auth;
using BlissShop.Common.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;

    public GoogleAuthController(IGoogleAuthService googleAuthService)
    {
        _googleAuthService = googleAuthService;
    }

    /// <summary>
    /// Register user with google.
    /// </summary>
    /// <param name="authorizationCode"></param>
    /// <param name="role"></param>
    /// <returns> This endpoint returns an access token and refresh token.</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(AuthSuccessDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleSignUp([FromHeader(Name = "Authorization-Code")] string authorizationCode, string role)
    {
        var result = await _googleAuthService.GoogleSignUpAsync(authorizationCode, role);
        return Ok(result);
    }

    /// <summary>
    /// Register user with google.
    /// </summary>
    /// <param name="authorizationCode"></param>
    /// <returns> This endpoint returns an access token and refresh token.</returns>
    [HttpPost("[action]")]
    [ProducesResponseType(typeof(AuthSuccessDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GoogleSignIn([FromHeader(Name = "Authorization-Code")] string authorizationCode)
    {
        var result = await _googleAuthService.GoogleSignInAsync(authorizationCode);
        return Ok(result);
    }
}
