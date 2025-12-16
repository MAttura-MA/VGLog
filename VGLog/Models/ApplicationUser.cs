using Microsoft.AspNetCore.Identity;

namespace VGLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }

    }
}
