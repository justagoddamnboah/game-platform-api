using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IWishlistService {
    Task<Wishlist> GetAllByUserIdAsync(Guid userId);
}