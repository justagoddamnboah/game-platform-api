namespace game_platform.net.dto.request;

public record CreatePurchaseRequest {
    public Guid UserId { get; init; }
    public IReadOnlyList<Guid> GameIds { get; init; } = [];
}