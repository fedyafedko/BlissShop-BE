using BlissShop.Abstraction.Auth;
using BlissShop.BLL.Services;
using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlissShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpPost("[action]")]
        public async Task<IActionResult> SignUp(SignUpDTO dto)
        {
            var result = await _authService.SignUpAsync(dto);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SignIn(SignInDTO dto)
        {
            var result = await _authService.SignInAsync(dto);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO dto)
        { 
            var result = await _emailConfirmationService.ConfirmEmailAsync(dto);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO dto)
        {
            var result = await _refreshTokenService.RefreshTokenAsync(dto);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _passwordService.ForgotPasswordAsync(request);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _passwordService.ResetPasswordAsync(request);
            return Ok(result);
        }
    }
}
