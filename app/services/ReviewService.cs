using game_platform.net.database;
using game_platform.net.dto.request;
using game_platform.net.interfaces;
using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.services;

public class ReviewService(PlatformDbContext db) : IReviewService {
    public async Task<IReadOnlyList<Review>> GetAllAsync()
        => await db.Reviews
            .AsNoTracking()
            .ToListAsync();

    
    public async Task<Review?> GetByIdAsync(Guid id)
        => await db.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    
    public async Task<Review> AddAsync(CreateReviewRequest request) {
        ValidateProductFields(request.Rating);
        var id = request.Id ?? Guid.NewGuid();
        if (await db.Games.AnyAsync(x => x.Id == id)) {
            throw new InvalidOperationException($"Отзыв с идентификатором {id} уже существует.");
        }
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null) {
            throw new InvalidOperationException("Пользователь не найден.");
        }
        var game = await db.Games.FirstOrDefaultAsync(x => x.Id == request.GameId);
        if (game is null) {
            throw new InvalidOperationException("Игра не найдена.");
        }
        var entity = new Review {
            Id = id,
            GameId = game.Id,
            UserId = user.Id,
            Rating = request.Rating,
            Commentary = request.Commentary,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Reviews.Add(entity);
        game.Reviews = game.Reviews.Append(entity.Id).ToArray();
        user.Reviews = user.Reviews.Append(entity.Id).ToArray();
        await db.SaveChangesAsync();
        game.Rating = await CalculateRating(entity.GameId);
        await db.SaveChangesAsync();
        return entity;
    }

    
    public async Task<Review?> UpdateAsync(Guid id, UpdateReviewRequest request) {
        ValidateProductFields(request.Rating);
        var entity = await db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null) {
            return null;
        }
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null) {
            throw new InvalidOperationException("Пользователь не найден.");
        }
        var game = await db.Games.FirstOrDefaultAsync(x => x.Id == request.GameId);
        if (game is null) {
            throw new InvalidOperationException("Игра не найдена.");
        }
        entity.GameId = game.Id;
        entity.UserId = user.Id;
        entity.Rating = request.Rating;
        entity.Commentary = request.Commentary;
        entity.CreatedAtUtc = DateTime.UtcNow;
        game.Reviews = game.Reviews.Append(entity.Id).ToArray();
        user.Reviews = user.Reviews.Append(entity.Id).ToArray();
        await db.SaveChangesAsync();
        game.Rating = await CalculateRating(entity.GameId);
        await db.SaveChangesAsync();
        return entity;
    }

    
    public async Task<bool> DeleteAsync(Guid id) {
        var entity = await db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        var game = await db.Games.FirstOrDefaultAsync(x => x.Id == entity.GameId);
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == entity.UserId);
        if (entity is null) {
            return false;
        }
        db.Reviews.Remove(entity);
        game.Reviews = game.Reviews.Where(reviewId => !entity.Id.Equals(reviewId)).ToArray();
        user.Reviews = user.Reviews.Where(reviewId => !entity.Id.Equals(reviewId)).ToArray();
        await db.SaveChangesAsync();
        game.Rating = await CalculateRating(entity.GameId);
        await db.SaveChangesAsync();
        return true;
    }

    
    private static void ValidateProductFields(int rating) {
        if ((rating < 0) || (rating > 5)) {
            throw new ArgumentException("Оценка должна быть от 0 до 5.");
        }
    }

    private async Task<float> CalculateRating(Guid gameId) {
        var reviews = await db.Reviews.AsNoTracking().Where(r => r.GameId == gameId).ToListAsync();
        float average = 0f;
        foreach (var review in reviews) {
            average += review.Rating;
        }
        if (reviews.Count == 0) {
            return 0f;
        }
        return average / reviews.Count;
    }
}