using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VGLog.Models;

namespace VGLog.Data
{
    public class AppDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    

    public DbSet<Videogame> Videogames { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    public DbSet<SoftwareHouse> SoftwareHouses {  get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Videogame>()
                .HasMany(v => v.Genres)
                .WithMany(g => g.Videogames);

            modelBuilder.Entity<Videogame>()
               .HasMany(v => v.Platforms)
               .WithMany(p => p.Videogames);

            modelBuilder.Entity<Videogame>()
                .HasOne(v => v.SoftwareHouse)
                .WithMany(sh => sh.Videogames)
                .HasForeignKey(v => v.SoftwareHouseId);

            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.Videogame)
                .WithMany(u => u.UserGames)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.Videogame)
                .WithMany(v => v.UserGames)
                .HasForeignKey(ug => ug.VideogameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGame>()
                .HasIndex(ug => new { ug.UserId, ug.VideogameId })
                .IsUnique();
        }
            
    }
}
