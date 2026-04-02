using VGLog.Models.Enums;

namespace VGLog.Services.DTOs
{
    public class UpdateUserGameDto
    {
        public int Id { get; set; }
        public int? HoursPlayed { get; set; }
        public string? Notes { get; set; }
        public int? PersonalRating { get; set; }

        public GameStatusEnum GameStatus { get; set; }
    }
}
