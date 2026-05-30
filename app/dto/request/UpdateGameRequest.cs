namespace game_platform.net.dto.request;

public class UpdateGameRequest {
    public Guid? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Age { get; init; }
}