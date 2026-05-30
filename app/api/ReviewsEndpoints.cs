using game_platform.net.dto;
using game_platform.net.dto.request;
using game_platform.net.dto.response;
using game_platform.net.interfaces;

namespace game_platform.net.api;

public static class ReviewsEndpoints {
    public static RouteGroupBuilder MapReviewsEndpoints(this RouteGroupBuilder api) {
        var group = api.MapGroup("/reviews").WithTags("Reviews");

        group.MapGet("/", async (IReviewService reviews, IMapper mapper) => {
                var result = await reviews.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список отзывов")
            .WithDescription("Возвращает весь список отзывов.")
            .Produces<IEnumerable<ReviewResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{reviewId:guid}",
                async (Guid reviewId, IReviewService reviews, IMapper mapper) => {
                var review = await reviews.GetByIdAsync(reviewId);
                return review is null
                    ? Results.NotFound(new ErrorResponse { Message = "Отзыв не найден." })
                    : Results.Ok(mapper.Map(review));
            })
            .WithSummary("Получить отзыв по идентификатору")
            .Produces<ReviewResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateReviewRequest body, IReviewService reviews, IMapper mapper) => {
                try {
                    var created = await reviews.AddAsync(body);
                    return Results.Created($"/api/reviews/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить отзыв")
            .Produces<ReviewResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{reviewId:guid}",
                async (Guid reviewId, UpdateReviewRequest body, IReviewService reviews, IMapper mapper) => {
                try {
                    var updated = await reviews.UpdateAsync(reviewId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Отзыв не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (ArgumentException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Изменить отзыв")
            .Produces<ReviewResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{reviewId:guid}",
                async (Guid reviewId, IReviewService reviews) => {
                try {
                    var deleted = await reviews.DeleteAsync(reviewId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Отзыв не найден." });
                }
                catch (InvalidOperationException ex) {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить отзыв")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return api;
    }
}