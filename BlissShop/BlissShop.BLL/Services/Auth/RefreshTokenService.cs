using BlissShop.Abstraction.Auth;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Entities;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlissShop.BLL.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public RefreshTokenService(
        UserManager<User> userManager,
        ITokenService tokenService,
        TokenValidationParameters tokenValidationParameters)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _tokenValidationParameters = tokenValidationParameters;
    }
    public async Task<AuthSuccessDTO> RefreshTokenAsync(RefreshTokenDTO dto)
    {
        var option = GetPrincipalFromToken(dto.AccessToken);
        var expiryDateUnix =
                    long.Parse(option.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix);

        if (expiryDateTimeUtc > DateTime.UtcNow)
            throw new IncorrectParametersException("Access token is not expired yet");

        var user = await _userManager.FindByIdAsync(option.Claims.Single(x => x.Type == "id").Value);
        if (user is null)
            throw new IncorrectParametersException("User with this id does not exist");

        if (DateTimeOffset.UtcNow > user.RefreshTokenExpiresAt)
            throw new ExpiredException("Refresh token is expired");

        if (user.RefreshToken != dto.RefreshToken)
            throw new IncorrectParametersException("Refresh token is invalid");

        return await _tokenService.GetAuthTokensAsync(user);
    }

    private ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = _tokenValidationParameters.Clone();
        validationParameters.ValidateLifetime = false;

        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        if (!HasValidSecurityAlgorithm(validatedToken))
            throw new InvalidSecurityAlgorithmException("Current token does not have right security algorithm");

        return principal;
    }

    private bool HasValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return validatedToken is JwtSecurityToken jwtSecurityToken &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                   StringComparison.InvariantCultureIgnoreCase);
    }
}
