using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;

class ImportImages
{
    static async Task Main(string[] args)
    {
        var apiKey = "5f0f6b2c1b274f91a1b776132ac6bb60";
        var httpClient = new HttpClient();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=C:\\Users\\Marco\\source\\repos\\VGLog\\VGLog\\VGLog.db")
            .Options;

        using var db = new AppDbContext(options);

        var games = await db.Videogames.ToListAsync();

        foreach (var game in games)
        {
            try
            {
                Console.WriteLine($"Cercando immagine per: {game.Title}");

                var searchUrl = $"https://api.rawg.io/api/games?key={apiKey}&search={Uri.EscapeDataString(game.Title)}&page_size=1";
                var response = await httpClient.GetStringAsync(searchUrl);

                var json = JsonDocument.Parse(response);
                var results = json.RootElement.GetProperty("results");

                if (results.GetArrayLength() == 0)
                {
                    Console.WriteLine($"  ⚠ Nessun risultato per {game.Title}");
                    continue;
                }

                var imageUrl = results[0].GetProperty("background_image").GetString();

                if (string.IsNullOrEmpty(imageUrl))
                {
                    Console.WriteLine($"  ⚠ Immagine non disponibile per {game.Title}");
                    continue;
                }

                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                game.Image = imageBytes;

                Console.WriteLine($"  ✓ Immagine inserita per {game.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Errore per {game.Title}: {ex.Message}");
            }
        }

        await db.SaveChangesAsync();
        Console.WriteLine("\nFatto! Tutte le immagini sono state salvate.");
    }
}
