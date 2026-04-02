using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VGLog.Models.Enums;

namespace VGLog.Models
{
    public class Friendship
    {
        public int Id { get; set; }


        [Required]
        public string UserRequesterId { get; set; }

        [ForeignKey(nameof(UserRequesterId))]
        public ApplicationUser UserRequester { get; set; } = null!;


        [Required]
        public string UserReceiverId { get; set; }

        [ForeignKey(nameof(UserReceiverId))]
        public ApplicationUser UserReceiver { get; set; } = null!;


        public FriendshipStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
