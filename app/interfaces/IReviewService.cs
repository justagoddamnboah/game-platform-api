using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IReviewService {
    Task<Review> GetAllReviewsByUserIdAsync(Guid userId);
    Task<Game> GetAllGamesByUserIdAsync(Guid userId);
}