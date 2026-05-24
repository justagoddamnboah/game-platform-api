using game_platform.net.dto.response;
using game_platform.net.model;

namespace game_platform.net.dto;

public class Mapper : IMapper {
    public UserResponse Map(User user) => new() {
        Id = user.Id,
        ProfileName = user.ProfileName,
        Email = user.Email
    };

    public UserLibraryResponse MapLibrary(User user, Purchase purchase) => new() {
        UserId = user.Id,
        GameIds = purchase.GameIds
    };

    public UserReviewResponse MapReviews(Review review, User user, Game game) => new() {
        Id = review.Id,
        UserId = user.Id,
        GameId = game.Id,
        Rating = review.Rating,
        Commentary = review.Commentary
    };

    public UserWishlistResponse MapWishlist(User user, Wishlist wishlist) => new() {
        UserId = user.Id,
        GameIds = wishlist.GameIds
    };
}