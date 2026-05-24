using game_platform.net.dto.response;
using game_platform.net.model;

namespace game_platform.net.dto;

public interface IMapper {
    UserResponse Map(User user);
    UserLibraryResponse MapLibrary(User user, Purchase purchase);
    UserReviewResponse MapReviews(Review review, User user, Game game);
    UserWishlistResponse MapWishlist(User user, Wishlist wishlist);
}