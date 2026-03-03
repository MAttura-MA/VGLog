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
        public int Dropped { get; set; }
        public int? TotalHours { get; set; }
        public int PersonalRating { get; set; }
        public double CompletionRate { get; set; }
        public double AvgRating { get; set; }
        public UserGame? MostPlayedGame { get; set; }
        public UserGame? MostRecentlyCompletedGame { get; set; }

    }
}
