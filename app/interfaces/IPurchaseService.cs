using game_platform.net.model;

namespace game_platform.net.interfaces;

public interface IPurchaseService {
    Task<Purchase> GetPurchasesByUserIdAsync(Guid userId);
}