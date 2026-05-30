using game_platform.net.dto.request;
using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IUserService {
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User> AddAsync(CreateUserRequest request);
    Task<User?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteAsync(Guid id);
}