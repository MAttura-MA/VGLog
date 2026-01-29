using VGLog.Models;

namespace VGLog.Services.DTOs
{
    public class AddUserGameDto
    {
        public int VideogameId { get; set; }
        public GameStatus Status { get; set; }

    }
}
