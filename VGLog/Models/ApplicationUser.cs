using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;

namespace VGLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        public byte[]? Avatar { get; set; }

        public string? AvatarPath { get; set; }

        public DateTime UserCreationTime { get; set; } = DateTime.Now;
        public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

    }
}
