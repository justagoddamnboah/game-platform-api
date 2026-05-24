namespace game_platform.net.dto.response;

public record UserWishlistResponse {
    public Guid UserId { get; init; }
    public Guid[]? GameIds { get; init; }
}