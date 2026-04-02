using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VGLog.Models;

namespace VGLog.Data
{
    //l'AppDbContext fa da ponte tra c# e il db, eredita da IdentityDbContext per includere automaticamente tutte le tabelle per la gestione delle identity,
    //mentre application user è la mia classe custom usata per aggiungere proprietà (come il displayname)
    public class AppDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    

        //dbset rappresenta una tabella nel db e permette di eseguire query LINQ
    public DbSet<Videogame> Videogames { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    public DbSet<SoftwareHouse> SoftwareHouses {  get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Friendship> Friendships { get; set; }

        //metodo chiamato  alla costruzione del modello del database, mappa tutte le relazioni tra le entità
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // n:n con tabella intermedia
            modelBuilder.Entity<Videogame>()
                .HasMany(v => v.Genres)
                .WithMany(g => g.Videogames);

            modelBuilder.Entity<Videogame>()
               .HasMany(v => v.Platforms)
               .WithMany(p => p.Videogames);

            // 1:n, un gioco ha una SoftwareHouse, una SoftwareHouse ha molti giochi, HasForeignKey determina quale proprietà sia la FK
            modelBuilder.Entity<Videogame>()
                .HasOne(v => v.SoftwareHouse)
                .WithMany(sh => sh.Videogames)
                .HasForeignKey(v => v.SoftwareHouseId);

            //1:1
            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.Videogame)
                .WithMany(v => v.UserGames)
                .HasForeignKey(ug => ug.VideogameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGame>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGames)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //creazione di un indice unico, un utente non può aggiungere lo stesso gioco due votle,
            modelBuilder.Entity<UserGame>()
                .HasIndex(ug => new { ug.UserId, ug.VideogameId })
                .IsUnique();

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserRequester)
                .WithMany()
                .HasForeignKey(f => f.UserRequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.UserReceiver)
                .WithMany()
                .HasForeignKey(f => f.UserReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasIndex(f => new { f.UserRequesterId, f.UserReceiverId })
                .IsUnique();
        }
            
    }
}
