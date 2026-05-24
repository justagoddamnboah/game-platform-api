namespace game_platform.net.dto.request;

public record CreateGameRequest {
    public Guid? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}