using game_platform.net.dto.response;
using game_platform.net.model;

namespace game_platform.net.dto;

public interface IMapper {
    UserResponse Map(User user);
    UserLibraryResponse MapLibrary(User user);
    GameResponse Map(Game game);
    ReviewResponse Map(Review review);
    PurchaseResponse Map(Purchase purchase);
}