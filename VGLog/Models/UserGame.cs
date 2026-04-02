using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VGLog.Models.Enums;

namespace VGLog.Models
{
    public class UserGame
    {
        public int Id { get; set; }

        [Required]
        public string UserId {  get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int VideogameId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Videogame Videogame { get; set; } = null!;

        public int? PersonalRating { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }

        public DateTime? TimeStampAdded { get; set; } = DateTime.Now;

        public GameStatusEnum GameStatus { get; set; }

        public int? HoursPlayed { get; set; }
    }
}
