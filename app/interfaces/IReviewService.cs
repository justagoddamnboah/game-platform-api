using game_platform.net.dto.request;
using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IReviewService {
    Task<IReadOnlyList<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(Guid id);
    Task<Review> AddAsync(CreateReviewRequest request);
    Task<Review?> UpdateAsync(Guid id, UpdateReviewRequest request);
    Task<bool> DeleteAsync(Guid id);
}