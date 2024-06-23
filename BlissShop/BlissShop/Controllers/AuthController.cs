using BlissShop.Abstraction.Auth;
using BlissShop.Common.DTO.Address;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Requests;
using BlissShop.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IPasswordService _passwordService;

        public AuthController(
            IAuthService authService,
            IEmailConfirmationService emailConfirmationService,
            IRefreshTokenService refreshTokenService,
            IPasswordService passwordService)
        {
            _authService = authService;
            _emailConfirmationService = emailConfirmationService;
            _refreshTokenService = refreshTokenService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Register user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns an userId.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp(SignUpDTO dto)
        {
            var result = await _authService.SignUpAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Login user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns an access token and refresh token.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthSuccessDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn(SignInDTO dto)
        {
            var result = await _authService.SignInAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Confirmation email for user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns an access token and refresh token.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthSuccessDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO dto)
        { 
            var result = await _emailConfirmationService.ConfirmEmailAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Resend confirmation email code.
        /// </summary>
        /// <param name="userId"></param>
        [HttpPut("[action]")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendConfirmationCode(Guid userId)
        {
            await _emailConfirmationService.ResendConfirmationCodeAsync(userId);
            return Ok();
        }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns> This endpoint returns an access token and refresh token.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthSuccessDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
        {
            var result = await _refreshTokenService.RefreshTokenAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _passwordService.ForgotPasswordAsync(request);
            return result ? Ok() : BadRequest();
        }

        /// <summary>
        /// Reset Password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> This endpoint returns a status code.</returns>
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(StatusCodes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _passwordService.ResetPasswordAsync(request);
            return result ? Ok() : BadRequest();
        }
    }
}
