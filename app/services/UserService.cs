using game_platform.net.database;
using game_platform.net.dto.request;
using game_platform.net.interfaces;
using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.services;

public class UserService(PlatformDbContext db) : IUserService {
    public async Task<IReadOnlyList<User>> GetAllAsync()
        => await db.Users
            .AsNoTracking()
            .OrderBy(x => x.ProfileName)
            .ToListAsync();

    public async Task<User?> GetByIdAsync(Guid id)
        => await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Game>> SeeLibrary(Guid userId) {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);
        var games = await db.Games
            .AsNoTracking()
            .Where(g => user.Library.Contains(g.Id))
            .ToListAsync();
        return games;
    }

    public async Task<User> AddAsync(CreateUserRequest request) {
        ValidateUserFields(request.ProfileName, request.Email, request.Age);
        var id = request.Id ?? Guid.NewGuid();
        if (await db.Users.AnyAsync(x => x.Id == id)) {
            throw new InvalidOperationException($"Пользователь с идентификатором {id} уже существует.");
        }
        var entity = new User {
            Id = id,
            ProfileName = request.ProfileName.Trim(),
            Email = request.Email.Trim(),
            Library = []
        };
        db.Users.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<User?> UpdateAsync(Guid id, UpdateUserRequest request) {
        ValidateUserFields(request.ProfileName, request.Email, request.Age);
        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) {
            return null;
        }
        entity.ProfileName = request.ProfileName.Trim();
        entity.Email = request.Email.Trim();
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id) {
        var entity = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) {
            return false;
        }
        var hasPurchases = await db.Purchases.AnyAsync(p => p.UserId == id);
        if (hasPurchases) {
            throw new InvalidOperationException("Нельзя удалить пользователя: есть связанные покупки.");
        }
        db.Users.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private static void ValidateUserFields(string profileName, string email, int age) {
        if (string.IsNullOrWhiteSpace(profileName)) {
            throw new ArgumentException("Имя пользователя не должно быть пустым.");
        }
        if (string.IsNullOrWhiteSpace(email)) {
            throw new ArgumentException("Email не должен быть пустым.");
        }
        if (age < 0) {
            throw new ArgumentException("Возраст не может быть отрицательным.");
        }
    }
}