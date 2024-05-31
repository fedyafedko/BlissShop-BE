using BlissShop.DAL.EF;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using BlissShop.Common.Configs;
using BlissShop.Middlewares;
using BlissShop.Seeding.Extentions;
using BlissShop.BLL.Profiles;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using BlissShop.Common.Extentions;
using BlissShop.FluentEmail.Services;
using BlissShop.Abstraction.FluentEmail;
using BlissShop.Abstraction.Auth;
using BlissShop.BLL.Services.Auth;
using BlissShop.Utility;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using BlissShop.Abstraction.Users;
using BlissShop.BLL.Services.Users;
using Microsoft.Extensions.FileProviders;
using FluentValidation;
using FluentValidation.AspNetCore;
using BlissShop.Validation.Auth;
using BlissShop.BLL.Services;
using BlissShop.Abstraction.Shop;
using BlissShop.Abstraction.Product;
using Stripe;
using BlissShop.Abstraction;
using BlissShop.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Configs
builder.Services.ConfigsAssembly(builder.Configuration, opt => opt
       .AddConfig<JwtConfig>()
       .AddConfig<EmailConfig>()
       .AddConfig<GoogleAuthConfig>()
       .AddConfig<CallbackUrisConfig>()
       .AddConfig<UserAvatarConfig>()
       .AddConfig<ShopAvatarConfig>()
       .AddConfig<ProductImagesConfig>()
       .AddConfig<StripeConfig>()
       .AddConfig<CategoryAvatarConfig>()
       .AddConfig<AuthConfig>());

builder.Services.AddAutoMapper(typeof(AuthProfile));

builder.Services.AddControllers(cfg => cfg.Filters.Add(typeof(ExceptionFilter)));
builder.Services.AddControllers(options => options
    .Conventions
    .Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, BlissShop.BLL.Services.Auth.TokenService>();
builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IProductService, BlissShop.BLL.Services.ProductService>();
builder.Services.AddScoped<IProductCartService, ProductCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

// Payment
StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeConfig:SecretKey");

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Fluent Email
builder.Services.FluentEmail(builder.Configuration);

// Seeding
builder.Services.AddSeeding();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<SignUpValidator>();
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtConfig:Secret")!)),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true
};
builder.Services.AddAuthentication(configureOptions: x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddSingleton(tokenValidationParameters);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
    c.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
});

// CORS
builder.Services.AddCors(options => options
    .AddDefaultPolicy(build => build
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

var app = builder.Build();

await app.ApplySeedingAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.MigrateDatabase();

app.UseHttpsRedirection();
app.UseCors(
    opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
