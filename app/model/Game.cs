namespace game_platform.net.model;

public class Game {
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
    
    public int AgeRestriction { get; set; }
    
    public int Sold { get; set; }
    
    public Review[] Reviews { get; set; }
    
    public float Rating { get; set; }
}