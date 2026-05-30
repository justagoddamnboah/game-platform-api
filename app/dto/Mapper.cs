using game_platform.net.dto.response;
using game_platform.net.model;

namespace game_platform.net.dto;

public class Mapper : IMapper {
    public UserResponse Map(User user) => new() {
        Id = user.Id,
        ProfileName = user.ProfileName,
        Email = user.Email,
        Age = user.Age,
        GamesCount = user.Library.Length
    };

    public UserLibraryResponse MapLibrary(User user) => new() {
        UserId = user.Id,
        GamesCount = user.Library.Length,
        GameIds = user.Library
    };
    
    public GameResponse Map(Game game) => new() {
        Id = game.Id,
        Name = game.Name,
        Price = game.Price,
        AgeRestriction = game.AgeRestriction
    };

    public ReviewResponse Map(Review review) => new() {
        Id = review.Id,
        GameId = review.GameId,
        UserId = review.UserId,
        Rating = review.Rating,
        Commentary = review.Commentary,
        CreatedAtUtc = review.CreatedAtUtc
    };

    public PurchaseResponse Map(Purchase purchase) => new() {
        Id = purchase.Id,
        UserId = purchase.UserId,
        GameIds = purchase.GameIds,
        Total = purchase.Total,
        CreatedAtUtc = purchase.CreatedAtUtc
    };
}