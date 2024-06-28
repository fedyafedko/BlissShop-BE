using AutoMapper;
using BlissShop.Abstraction;
using BlissShop.Common.DTO.Rating;
using BlissShop.Common.Exceptions;
using BlissShop.DAL.Repositories.Interfaces;
using BlissShop.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlissShop.BLL.Services;

public class RatingService : IRatingService
{
    private readonly UserManager<User> _userManager;
    private readonly IRepository<Rating> _ratingRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public RatingService(
        UserManager<User> userManager,
        IRepository<Rating> ratingRepository,
        IRepository<Product> productRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _ratingRepository = ratingRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<RatingDTO> AddRatingAsync(Guid userId, CreateRatingDTO dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var product = await _productRepository.FirstOrDefaultAsync(x => x.Id == dto.ProductId)
            ?? throw new NotFoundException("Product not found");
        
        var entity = _mapper.Map<Rating>(dto);
        entity.UserId = user.Id;

        await _ratingRepository.InsertAsync(entity);

        return _mapper.Map<RatingDTO>(entity);
    }

    public async Task<List<RatingDTO>> GetRatingForProductAsync(Guid productId)
    {
        var product = await _productRepository
            .Include(x => x.Ratings)
            .FirstOrDefaultAsync(x => x.Id == productId)
            ?? throw new NotFoundException("Product not found");

        

        return _mapper.Map<List<RatingDTO>>(product.Ratings);
    }

    public async Task<List<RatingDTO>> GetRatingForUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User not found");

        var ratings = await _ratingRepository
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        return _mapper.Map<List<RatingDTO>>(ratings);
    }
}
