using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VGLog.Models
{
    public class Videogame
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public byte[]? Image { get; set; }

        public List<Genre> Genres { get; set; } = new();
        public List<Platform> Platforms { get; set; } = new();

        public int? SoftwareHouseId {  get; set; }
        public SoftwareHouse? SoftwareHouse { get; set; }

        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
    }
}
