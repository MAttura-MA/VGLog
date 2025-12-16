using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VGLog.Models
{
    public class Videogame
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public byte? Image { get; set; }
        public GameStatus Status { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt {  get; set; }
        public int? PersonalRating { get; set; }
        public string? Notes { get; set; }

        public List<Genre> Genres { get; set; } = new();
        public List<Platform> Platforms { get; set; } = new();

        public int? SoftwareHouseId {  get; set; }
        public SoftwareHouse? SoftwareHouse { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
