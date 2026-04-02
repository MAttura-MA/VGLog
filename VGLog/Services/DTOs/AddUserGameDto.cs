using VGLog.Models.Enums;

namespace VGLog.Services.DTOs
{
    public class AddUserGameDto
    {
        public int VideogameId { get; set; }
        public GameStatusEnum Status { get; set; }

    }
}
