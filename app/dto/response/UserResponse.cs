namespace game_platform.net.dto.response;

public record UserResponse {
    public Guid Id { get; init; }
    public string ProfileName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    //public int GamesCount { get; init; }
}