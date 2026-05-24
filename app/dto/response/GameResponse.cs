namespace game_platform.net.dto.response;

public record GameResponse {
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}