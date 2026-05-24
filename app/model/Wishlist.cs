namespace game_platform.net.model;

public class Wishlist {
    public Guid UserId { get; set; }
    public Guid[]? GameIds { get; set; }
}