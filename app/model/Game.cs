namespace game_platform.net.model;

public class Game {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AgeRestriction { get; set; }
    public Guid[] Reviews { get; set; } = new Guid[] { };
}