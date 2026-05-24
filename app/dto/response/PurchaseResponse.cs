namespace game_platform.net.dto.response;

public record PurchaseResponse {
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName {  get; init; } = string.Empty;
    public Guid[] GameIds { get; init; } = [];
    public string[] GameNames { get; set; } = [];
    public decimal Total { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}