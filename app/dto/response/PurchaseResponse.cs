namespace game_platform.net.dto.response;

public record PurchaseResponse {
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid[] GameIds { get; init; } = [];
    public decimal Total { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}