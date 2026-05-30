namespace game_platform.net.model;

public class User {
    public Guid Id { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
	public int Age { get; set; }
    public Guid[] Library { get; set; } = new Guid[] { };
    public Guid[] Reviews { get; set; } = new Guid[] { };
}