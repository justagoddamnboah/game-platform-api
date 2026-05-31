using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.database;

public class PlatformDbContext(DbContextOptions<PlatformDbContext> options) : DbContext(options) {
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.ProfileName).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(300).IsRequired();
        });
        modelBuilder.Entity<Game>(entity => {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).HasMaxLength(200).IsRequired();
        });
        modelBuilder.Entity<Review>(entity => {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Commentary).HasMaxLength(1000).IsRequired();
        });
    }
}