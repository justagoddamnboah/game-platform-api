using game_platform.net.dto;
using game_platform.net.dto.request;
using game_platform.net.dto.response;
using game_platform.net.interfaces;

namespace game_platform.net.api;

public static class UsersEndpoints {
    public static RouteGroupBuilder MapUsersEndpoints(this RouteGroupBuilder api) {
        var group = api.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (IUserService users, IMapper mapper) => {
                var result = await users.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список пользователей")
            .WithDescription("Возвращает всех зарегистрированных пользователей и количество купленных ими игр.")
            .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK);

        
        
        group.MapGet("/{userId:guid}",
                async (Guid userId, IUserService users, IMapper mapper) => {
                    var user = await users.GetByIdAsync(userId);
                    return user is null
                        ? Results.NotFound(new ErrorResponse { Message = "Пользователь не найден."})
                        : Results.Ok(mapper.Map(user));
                })
            .WithSummary("Получить пользователя по идентификатору")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        group.MapGet("/{userId:guid}/library",
                async (Guid userId, IUserService users, IMapper mapper) => {
                    var user = await users.GetByIdAsync(userId);
                    if (user == null) {
                        return Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." });
                    }
                    var games = await users.SeeLibrary(userId);
                    return games.Count == 0
                        ? Results.NotFound(new ErrorResponse {Message = "Библиотека пуста."})
                        : Results.Ok(mapper.MapLibrary(user));
                })
            .WithSummary("Посмотреть библиотеку игр пользователя")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        group.MapGet("/{userId:guid}/reviews",
                async (Guid userId, IUserService users, IMapper mapper) => {
                    var user = await users.GetByIdAsync(userId);
                    if (user == null) {
                        return Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." });
                    }
                    var reviews = await users.SeeUsersReviews(userId);
                    var reviewResponses = reviews.Select(review => mapper.Map(review)).ToList();
                    return reviews.Count == 0
                        ? Results.NotFound(new ErrorResponse {Message = "Отзывов нет."})
                        : Results.Ok(reviewResponses);
                })
            .WithSummary("Посмотреть все отзывы пользователя")
            .Produces<ReviewResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
        
        
        group.MapPost("/", async (CreateUserRequest body, IUserService users, IMapper mapper) => {
                try {
                    var created = await users.AddAsync(body);
                    return Results.Created($"/api/users/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить пользователя")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{userId:guid}",
                async (Guid userId, UpdateUserRequest body, IUserService users, IMapper mapper) => {
                try {
                    var updated = await users.UpdateAsync(userId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить пользователя")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{userId:guid}",
                async (Guid userId, IUserService users) => {
                try {
                    var deleted = await users.DeleteAsync(userId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Пользователь не найден." });
                }
                catch (InvalidOperationException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить пользователя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}