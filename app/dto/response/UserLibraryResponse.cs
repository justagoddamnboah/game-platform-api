namespace game_platform.net.dto.response;

public record UserLibraryResponse {
    public Guid UserId { get; init; }
    //public int GamesCount { get; init; }
    public Guid[]? GameIds { get; init; }
}