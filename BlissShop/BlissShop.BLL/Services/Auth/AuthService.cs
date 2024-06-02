using AutoMapper;
using BlissShop.Abstraction.Auth;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Common.DTO.Auth;
using BlissShop.Common.Exceptions;
using BlissShop.Common.Extentions;
using BlissShop.Common.Responses;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using BlissShop.FluentEmail.MessageBase;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BlissShop.BLL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Setting> _settingRepository;
    private readonly IEmailConfirmationService _emailConfirmationService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<User> userManager,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IMapper mapper,
        IEmailConfirmationService emailConfirmationService,
        IEmailService emailService,
        IRepository<Setting> settingRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
        _emailConfirmationService = emailConfirmationService;
        _emailService = emailService;
        _settingRepository = settingRepository;
    }
    public async Task<RegisterResponse> SignUpAsync(SignUpDTO dto)
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

        var setting = new Setting { UserId = user.Id };

        await _settingRepository.InsertAsync(setting);

        var generatedCode = await _emailConfirmationService.GenerateEmailCodeAsync(user.Id);

        await _emailService.SendAsync(new ConfirmedEmailMessage { Recipient = user.Email!, Code = generatedCode });

        return new RegisterResponse { UserId = user.Id };
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
