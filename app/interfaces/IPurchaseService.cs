using game_platform.net.dto.request;
using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IPurchaseService {
    Task<IReadOnlyList<Purchase>> GetAllAsync();
    Task<Purchase?> GetByIdAsync(Guid id);
    Task<Purchase> CreateAsync(CreatePurchaseRequest request);
    Task<Purchase?> UpdateAsync(Guid id, CreatePurchaseRequest request);
    Task<bool> DeleteAsync(Guid id);
}