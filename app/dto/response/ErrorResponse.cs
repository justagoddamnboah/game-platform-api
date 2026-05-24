namespace game_platform.net.dto.response;

public record ErrorResponse {
    public string Message { get; init; } = string.Empty;
}