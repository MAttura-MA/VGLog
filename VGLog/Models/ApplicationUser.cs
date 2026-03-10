using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace VGLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        public byte[]? Avatar { get; set; }

        public string? AvatarPath { get; set; }

        [MaxLength(300)]
        public string? Bio { get; set; }

        public DateTime UserCreationTime { get; set; } = DateTime.Now;
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

    }
}
