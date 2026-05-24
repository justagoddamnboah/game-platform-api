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
            throw new ArgumentException("Список товаров в покупке не должен быть пустым.");
        }

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null) {
            throw new InvalidOperationException("Пользователь не найден.");
        }

        var groupedGameIds = request.GameIds
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        var products = await db.Games
            .Where(x => groupedGameIds.Keys.Contains(x.Id))
            .ToListAsync();

        foreach (var pair in groupedGameIds) {
            var game = products.FirstOrDefault(x => x.Id == pair.Key);
            if (game is null) {
                throw new InvalidOperationException($"Товар {pair.Key} не найден.");
            }
        }

        var prices = products.ToDictionary(x => x.Id, x => x.Price);
        var total = request.GameIds.Sum(productId => prices[productId]);

        var purchase = new Purchase {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            UserName = user.ProfileName,
            CreatedAtUtc = DateTime.UtcNow,
            Total = total,
            GameIds = request.GameIds.ToArray()
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
            throw new InvalidOperationException("Клиент не найден.");
        }

        var groupedGameIds = request.GameIds
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());

        var games = await db.Games
            .Where(x => groupedGameIds.Keys.Contains(x.Id))
            .ToListAsync();

        foreach (var pair in groupedGameIds) {
            var game = games.FirstOrDefault(x => x.Id == pair.Key);
            if (game is null) {
                throw new InvalidOperationException($"Игра {pair.Key} не найдена.");
            }
        }

        var prices = games.ToDictionary(x => x.Id, x => x.Price);
        var total = request.GameIds.Sum(productId => prices[productId]);

        purchase.UserId = newCustomer.Id;
        purchase.UserName = newCustomer.ProfileName;
        purchase.GameIds = request.GameIds.ToArray();
        purchase.Total = total;

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