namespace game_platform.net.model;

public class Review {
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Commentary { get; set; }
}