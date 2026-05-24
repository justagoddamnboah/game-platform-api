namespace game_platform.net.model;

public class Purchase {
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid[] GameIds { get; set; } = [];
    
    public decimal Total { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
}