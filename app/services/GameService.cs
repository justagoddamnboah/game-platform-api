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
            .OrderBy(x => x.Name)
            .ToListAsync();

    public async Task<Game?> GetByIdAsync(Guid id)
        => await db.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Game> AddAsync(CreateGameRequest request) {
        ValidateProductFields(request.Name, request.Price);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Games.AnyAsync(x => x.Id == id)) {
            throw new InvalidOperationException($"Игра с идентификатором {id} уже существует.");
        }

        var entity = new Game {
            Id = id,
            Name = request.Name.Trim(),
            Price = request.Price
        };

        db.Games.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Game?> UpdateAsync(Guid id, UpdateGameRequest request)
    {
        ValidateProductFields(request.Name, request.Price);

        var entity = await db.Games.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) {
            return null;
        }

        entity.Name = request.Name.Trim();
        entity.Price = request.Price;
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id) {
        var entity = await db.Games.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) {
            return false;
        }

        db.Games.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateProductFields(string name, decimal price) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("Название игры не должно быть пустым.");
        }

        if (price < 0) {
            throw new ArgumentException("Цена не может быть отрицательной.");
        }
    }
}