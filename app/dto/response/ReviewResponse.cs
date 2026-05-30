namespace game_platform.net.dto.response;

public record ReviewResponse {
    public Guid? Id { get; init; }
    public Guid GameId { get; init; }
    public Guid UserId { get; init; }
    public int Rating { get; init; }
    public string? Commentary { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}