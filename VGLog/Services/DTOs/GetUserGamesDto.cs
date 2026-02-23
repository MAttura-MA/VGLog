using VGLog.Models;

namespace VGLog.Services.DTOs
{
    public class GetUserGamesDto
    {
        public List<UserGame> Games { get; set; } = new();

        public int Total { get; set; }
        public int Completed { get; set; }
        public int Playing { get; set; }
        public int ToPlay { get; set; }
        public int? TotalHours { get; set; }

    }
}
