using BlissShop.Abstraction;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Entities;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlissShop.BLL.Services;

public class TokenService : ITokenService
{
    private readonly JwtConfig _jwtConfig;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;


    public TokenService(
        IOptions<JwtConfig> jwtConfig,
        UserManager<User> userManager,
        ILogger<AuthService> logger)
    {
        _jwtConfig = jwtConfig.Value;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtConfig.AccessTokenLifeTime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        user.RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        user.RefreshTokenExpiresAt = DateTimeOffset.UtcNow.Add(_jwtConfig.RefreshTokenLifeTime);
        var userUpdated = await _userManager.UpdateAsync(user);
        if (!userUpdated.Succeeded)
            throw new UserManagerException("Exeption with updating user", userUpdated.Errors);

        return user.RefreshToken;
    }

    public async Task<AuthSuccessDTO> GetAuthTokensAsync(User user)
    {
        return new AuthSuccessDTO
        {
            AccessToken = await GenerateAccessTokenAsync(user),
            RefreshToken = await GenerateRefreshTokenAsync(user)
        };
    }
}
