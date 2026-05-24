using game_platform.net.database;
using game_platform.net.dto.request;
using game_platform.net.interfaces;
using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.services;

public class PurchaseService(PlatformDbContext db) : IPurchaseService {
    public async Task<IReadOnlyList<Purchase>> GetAllAsync()
        => await db.Purchases
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id)
        => await db.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Purchase> CreateAsync(CreatePurchaseRequest request) {
        if (request.GameIds.Count == 0) {
            throw new ArgumentException("Список игр в покупке не должен быть пустым.");
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null) {
            throw new InvalidOperationException("Пользователь не найден.");
        }

        var uniqueGameIds = request.GameIds.Distinct().ToList();

        if (uniqueGameIds.Count == 0) {
            throw new ArgumentException("Список игр в покупке не должен быть пустым.");
        }

        var games = await db.Games
            .Where(x => uniqueGameIds.Contains(x.Id))
            .ToListAsync();

        if (games.Count != uniqueGameIds.Count) {
            var foundGameIds = games.Select(g => g.Id).ToHashSet();
            var missingGameIds = uniqueGameIds.Where(id => !foundGameIds.Contains(id));
            throw new InvalidOperationException($"Следующие игры не найдены: {string.Join(", ", missingGameIds)}");
        }
        
        var total = games.Sum(game => game.Price);

        var gameNames = games.Select(g => g.Name).ToArray();

        var purchase = new Purchase {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            UserName = user.ProfileName,
            CreatedAtUtc = DateTime.UtcNow,
            Total = total,
            GameIds = uniqueGameIds.ToArray(),
            GameNames = gameNames
        };

        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        return purchase;
    }

    public async Task<Purchase?> UpdateAsync(Guid id, CreatePurchaseRequest request) {
        if (request.GameIds.Count == 0) {
            throw new ArgumentException("Список игр в покупке не должен быть пустым.");
        }

        var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null) {
            return null;
        }

        var newCustomer = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (newCustomer is null) {
            throw new InvalidOperationException("Пользователь не найден.");
        }

        var uniqueGameIds = request.GameIds.Distinct().ToList();

        if (uniqueGameIds.Count == 0) {
            throw new ArgumentException("Список игр в покупке не должен быть пустым.");
        }

        var games = await db.Games
            .Where(x => uniqueGameIds.Contains(x.Id))
            .ToListAsync();

        if (games.Count != uniqueGameIds.Count) {
            var foundGameIds = games.Select(g => g.Id).ToHashSet();
            var missingGameIds = uniqueGameIds.Where(id => !foundGameIds.Contains(id));
            throw new InvalidOperationException($"Следующие игры не найдены: {string.Join(", ", missingGameIds)}");
        }
        
        var total = games.Sum(game => game.Price);

        var gameNames = games.Select(g => g.Name).ToArray();

        purchase.UserId = newCustomer.Id;
        purchase.UserName = newCustomer.ProfileName;
        purchase.GameIds = request.GameIds.ToArray();
        purchase.Total = total;
        purchase.GameNames = gameNames;

        await db.SaveChangesAsync();
        return purchase;
    }

    public async Task<bool> DeleteAsync(Guid id) {
        var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null) {
            return false;
        }

        db.Purchases.Remove(purchase);
        await db.SaveChangesAsync();
        return true;
    }
}