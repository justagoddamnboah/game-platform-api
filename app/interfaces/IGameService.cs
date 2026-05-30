using game_platform.net.dto.request;
using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IGameService {
    Task<IReadOnlyList<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Review>> SeeGameReviews(Guid gameId);
    Task<Game> AddAsync(CreateGameRequest request);
    Task<Game?> UpdateAsync(Guid id, UpdateGameRequest request);
    Task<bool> DeleteAsync(Guid id);
}