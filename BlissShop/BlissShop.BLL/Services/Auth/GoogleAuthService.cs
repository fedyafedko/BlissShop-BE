using AutoMapper;
using BlissShop.Abstraction.Auth;
using BlissShop.Common.Configs;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Entities;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services.Auth;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly GoogleAuthConfig _googleConfig;
    private readonly ILogger<GoogleAuthService> _logger;
    private readonly IMapper _mapper;

    public GoogleAuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        GoogleAuthConfig googleConfig,
        ILogger<GoogleAuthService> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _googleConfig = googleConfig;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AuthSuccessDTO> GoogleSignUpAsync(string authorizationCode, string role)
    {
        var payload = await GetGooglePayloadAsync(authorizationCode);

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user != null)
            throw new NotFoundException("User with this email already exists");

        user = _mapper.Map<User>(payload);
        user.EmailConfirmed = true;

        var createdUserResult = await _userManager.CreateAsync(user);

        if (!createdUserResult.Succeeded)
        {
            _logger.LogIdentityErrors(user, createdUserResult);
            throw new UserManagerException("Unable to authenticate given user", createdUserResult.Errors);
        }

        await _userManager.AddToRoleAsync(user, role);

        return await _tokenService.GetAuthTokensAsync(user);
    }

    public async Task<AuthSuccessDTO> GoogleSignInAsync(string authorizationCode)
    {
        var paylod = await GetGooglePayloadAsync(authorizationCode);

        var user = await _userManager.FindByEmailAsync(paylod.Email)
            ?? throw new NotFoundException($"Unable to find user by specified email. Email: {paylod.Email}");

        return await _tokenService.GetAuthTokensAsync(user);
    }

    private async Task<GoogleJsonWebSignature.Payload> GetGooglePayloadAsync(string authorizationCode)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _googleConfig.ClientId,
                ClientSecret = _googleConfig.ClientSecret
            }
        });

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            string.Empty,
            authorizationCode,
            _googleConfig.RedirectUri,
            CancellationToken.None);

        var setting = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { _googleConfig.ClientId },
            IssuedAtClockTolerance = _googleConfig.IssuedAtClockTolerance,
            ExpirationTimeClockTolerance = _googleConfig.ExpirationTimeClockTolerance,
        };

        return await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken, setting);
    }
}
