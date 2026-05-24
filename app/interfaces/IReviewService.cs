using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IReviewService {
    Task<Review> GetReviewsByUserIdAsync(Guid userId);
    Task<Game> GetGamesByUserIdAsync(Guid userId);
}