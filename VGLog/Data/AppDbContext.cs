using Microsoft.EntityFrameworkCore;
using VGLog.Models;

namespace VGLog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    

    public DbSet<Videogame> Videogames { get; set; }
    public DbSet<SoftwareHouse> SoftwareHouses {  get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Platform> platforms { get; set; }

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
        }
            
    }
}
