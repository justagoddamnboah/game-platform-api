using game_platform.net.model;
using Microsoft.EntityFrameworkCore;

namespace game_platform.net.database;

public class PlatformDbContext(DbContextOptions<PlatformDbContext> options) : DbContext(options) {
    public DbSet<User> Users { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity => {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProfileName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(300).IsRequired();
        });
    }
}