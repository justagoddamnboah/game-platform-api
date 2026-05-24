using game_platform.net.dto.response;
using game_platform.net.model;

namespace game_platform.net.dto;

public class Mapper : IMapper {
    public UserResponse Map(User user) => new() {
        Id = user.Id,
        ProfileName = user.ProfileName,
        Email = user.Email
    };
    
    public GameResponse Map(Game game) => new() {
        Id = game.Id,
        Name = game.Name,
        Price = game.Price
    };

    public PurchaseResponse Map(Purchase purchase) => new() {
        Id = purchase.Id,
        UserId = purchase.UserId,
        UserName = purchase.UserName,
        GameIds = purchase.GameIds,
        Total = purchase.Total,
        CreatedAtUtc = purchase.CreatedAtUtc
    };
}