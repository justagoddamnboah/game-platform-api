using game_platform.net.dto;
using game_platform.net.dto.request;
using game_platform.net.dto.response;
using game_platform.net.interfaces;

namespace game_platform.net.api;

public static class GamesEndpoints {
    public static RouteGroupBuilder MapGamesEndpoints(this RouteGroupBuilder api) {
        var group = api.MapGroup("/games").WithTags("Games");

        group.MapGet("/", async (IGameService games, IMapper mapper) => {
                var result = await games.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список игр")
            .WithDescription("Возвращает весь доступный каталог игр.")
            .Produces<IEnumerable<GameResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{gameId:guid}",
                async (Guid gameId, IGameService games, IMapper mapper) => {
                var game = await games.GetByIdAsync(gameId);
                return game is null
                    ? Results.NotFound(new ErrorResponse { Message = "Игра не найдена." })
                    : Results.Ok(mapper.Map(game));
            })
            .WithSummary("Получить игру по идентификатору")
            .Produces<GameResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{gameId:guid}/reviews",
                async (Guid gameId, IGameService games, IMapper mapper) => {
                    var game = await games.GetByIdAsync(gameId);
                    if (game == null) {
                        return Results.NotFound(new ErrorResponse { Message = "Игра не найдена." });
                    }
                    var reviews = await games.SeeGameReviews(gameId);
                    var reviewResponses = reviews.Select(review => mapper.Map(review)).ToList();
                    return reviews.Count == 0
                        ? Results.NotFound(new ErrorResponse {Message = "Отзывов нет."})
                        : Results.Ok(reviewResponses);
                })
            .WithSummary("Посмотреть все отзывы игры")
            .Produces<ReviewResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        group.MapPost("/", async (CreateGameRequest body, IGameService games, IMapper mapper) => {
                try {
                    var created = await games.AddAsync(body);
                    return Results.Created($"/api/games/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить игру")
            .Produces<GameResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{gameId:guid}",
                async (Guid gameId, UpdateGameRequest body, IGameService games, IMapper mapper) => {
                try {
                    var updated = await games.UpdateAsync(gameId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Игра не найдена." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить игру")
            .Produces<GameResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{gameId:guid}",
                async (Guid gameId, IGameService games) => {
                try {
                    var deleted = await games.DeleteAsync(gameId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Игра не найдена." });
                }
                catch (InvalidOperationException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить игру")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}