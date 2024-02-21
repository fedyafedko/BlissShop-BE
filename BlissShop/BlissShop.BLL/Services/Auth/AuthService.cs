using AutoMapper;
using BlissShop.Abstraction.Auth;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services.Auth;

public class AuthService : IAuthService
{
    public readonly UserManager<User> _userManager;
    public readonly IEmailConfirmationService _emailConfirmationService;
    public readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    public readonly ILogger<AuthService> _logger;
    public readonly IMapper _mapper;

    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IMapper mapper,
        IEmailConfirmationService emailConfirmationService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
        _emailConfirmationService = emailConfirmationService;
        _emailService = emailService;
    }
    public async Task<RegisterResponseDTO> SignUpAsync(SignUpDTO dto)
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

        var generatedCode = await _emailConfirmationService.GenerateEmailCodeAsync(user.Id);

        await _emailService.SendAsync(user.Email!, new ConfirmedEmailMessage { Code = generatedCode });

        return new RegisterResponseDTO { UserId = user.Id };
    }

    public async Task<AuthSuccessDTO> SignInAsync(SignInDTO dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new NotFoundException($"Unable to find user by specified email. Email: {dto.Email}");

        if (!user.EmailConfirmed)
            throw new ConfirmedEmailException("Email is not confirmed");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordValid)
            throw new IncorrectParametersException($"User input incorrect password. Password: {dto.Password}");

        return await _tokenService.GetAuthTokensAsync(user);
    }
}
