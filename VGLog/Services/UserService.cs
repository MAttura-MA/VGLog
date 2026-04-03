using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services.Interfaces;

namespace VGLog.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> SearchUsersAsync(string query, string currentUserId)
        {
            return await _userManager.Users
                .Where(u => u.Id != currentUserId &&
                            u.UserName!.ToLower().Contains(query))
                .Take(8)
                .ToListAsync();
        }
    }
}
