using VGLog.Models;

namespace VGLog.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> SearchUsersAsync(string query, string currentUserId);
    }
}
