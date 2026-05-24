namespace game_platform.net.dto.response;

public record UserReviewResponse {
    public Guid Id { get; init; }
    public Guid GameId { get; init; }
    public Guid UserId { get; init; }
    public int Rating { get; init; }
    public string? Commentary { get; init; }
}