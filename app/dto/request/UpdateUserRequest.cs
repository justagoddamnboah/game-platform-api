namespace game_platform.net.dto.request;

public record UpdateUserRequest {
    public string ProfileName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}