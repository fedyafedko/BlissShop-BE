using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services;

public class AuthService : IAuthService
{
    public readonly UserManager<User> _userManager;
    public readonly ITokenService _tokenService;
    public readonly ILogger<AuthService> _logger;
    public readonly IMapper _mapper;

    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<AuthSuccessDTO> SignUpAsync(SignUpDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user != null)
            throw new AlreadyExistsException($"User with specified email already exists. Email: {dto.Email}");

        user = _mapper.Map<User>(dto);
        user.UserName = dto.Email;

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            _logger.LogIdentityErrors(user, result);
            throw new UserManagerException($"User manager operation failed:\n", result.Errors);
        }

        var role = dto.Role;

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            _logger.LogError($"Failed to add user to role. Role: {role}");
            throw new UserManagerException($"User manager operation failed:\n", result.Errors);
        }

        return await GetAuthTokensAsync(user);
    }

    public async Task<AuthSuccessDTO> SignInAsync(SignInDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new Exception($"Unable to find user by specified email. Email: {dto.Email}");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
            throw new Exception($"User input incorrect password. Password: {dto.Password}");

        return await GetAuthTokensAsync(user);
    }


    protected async Task<AuthSuccessDTO> GetAuthTokensAsync(User user)
    {
        return new AuthSuccessDTO
        {
            AccessToken = await _tokenService.GenerateAccessTokenAsync(user),
        };
    }
}
