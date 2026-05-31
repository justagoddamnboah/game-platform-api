using game_platform.net.database;
using game_platform.net.dto.request;
using game_platform.net.interfaces;
using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.services;

public class GameService(PlatformDbContext db) : IGameService {
    public async Task<IReadOnlyList<Game>> GetAllAsync()
        => await db.Games
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .ToListAsync();

    
    public async Task<Game?> GetByIdAsync(Guid id)
        => await db.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);
    
    
    public async Task<IReadOnlyList<Review>> SeeGameReviews(Guid gameId) {
        var game = await db.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == gameId);
        var reviews = await db.Reviews
            .AsNoTracking()
            .Where(r => game.Reviews.Contains(r.Id))
            .ToListAsync();
        return reviews;
    }

    
    public async Task<Game> AddAsync(CreateGameRequest request) {
        ValidateProductFields(request.Name, request.Price, request.AgeRestriction);
        var id = request.Id ?? Guid.NewGuid();
        if (await db.Games.AnyAsync(g => g.Id == id)) {
            throw new InvalidOperationException($"Игра с идентификатором {id} уже существует.");
        }
        var entity = new Game {
            Id = id,
            Name = request.Name.Trim(),
            Price = request.Price,
            AgeRestriction = request.AgeRestriction,
            Reviews = [],
            Rating = 0f
        };
        db.Games.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    
    public async Task<Game?> UpdateAsync(Guid id, UpdateGameRequest request) {
        ValidateProductFields(request.Name, request.Price, request.AgeRestriction);
        var entity = await db.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (entity is null) {
            return null;
        }
        entity.Name = request.Name.Trim();
        entity.Price = request.Price;
        entity.AgeRestriction = request.AgeRestriction;
        await db.SaveChangesAsync();
        return entity;
    }

    
    public async Task<bool> DeleteAsync(Guid id) {
        var entity = await db.Games.FirstOrDefaultAsync(g => g.Id == id);
        if (entity is null) {
            return false;
        }
        var usedInPurchase = await db.Purchases.AnyAsync(p => p.GameIds.Contains(id));
        if (usedInPurchase) {
            throw new InvalidOperationException("Нельзя удалить игру: она указана в одной или нескольких покупках.");
        }
        var hasReviews = await db.Reviews.AnyAsync(r => r.GameId == id);
        if (hasReviews) {
            throw new InvalidOperationException("Нельзя удалить игру: у нее есть отзывы.");
        }
        db.Games.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    
    private static void ValidateProductFields(string name, decimal price, int ageRestriction) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("Название игры не должно быть пустым.");
        }
        if (price < 0) {
            throw new ArgumentException("Цена не может быть отрицательной.");
        }
        if (ageRestriction < 0) {
            throw new ArgumentException("Возрастное ограничение не может быть отрицательным.");
        }
    }
}