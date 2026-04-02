using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;
using VGLog.Models.Enums;
using VGLog.Services.Interfaces;

namespace VGLog.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendshipService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SendRequestAsync(string requesterId, string receiverId)
        {
            var exists = await _context.Friendships.AnyAsync(f =>
            (f.UserRequesterId == requesterId && f.UserReceiverId == receiverId) ||
            (f.UserRequesterId == receiverId && f.UserReceiverId == requesterId));

            if (exists)
                return;

            _context.Friendships.Add(new Friendship
            {
                UserRequesterId = requesterId,
                UserReceiverId = receiverId,
                Status = FriendshipStatus.Pending
            });

            await _context.SaveChangesAsync();

        }

        public async Task AcceptRequestAsync( string currentUserId, string requesterId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
            f.UserRequesterId == requesterId &&
            f.UserReceiverId == currentUserId &&
            f.Status == FriendshipStatus.Pending);

            if (friendship is null)
                return;

            friendship.Status = FriendshipStatus.Accepted;

            await _context.SaveChangesAsync();

        }

        public async Task DeclineRequestAsync(string currentUserId, string requesterId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
            f.UserRequesterId == requesterId &&
            f.UserReceiverId == currentUserId);

            if (friendship is not null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();

            }
        }

        public async Task RemoveFriendAsync(string userId, string friendId)
        {
            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
            (f.UserRequesterId == userId && f.UserReceiverId == friendId) ||
            (f.UserRequesterId == friendId && f.UserReceiverId == userId));

            if (friendship is not null)
            {
                _context.Friendships.Remove(friendship);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<FriendshipStatus?> GetStatusAsync(string userA, string userB)
        {
            var f = await _context.Friendships.FirstOrDefaultAsync(f =>
            (f.UserRequesterId == userA && f.UserReceiverId == userB) ||
            (f.UserRequesterId == userB && f.UserReceiverId == userA));

            return f?.Status;
        }

        public async Task<List<ApplicationUser>> GetFriendsAsync(string userId)
        {
            return await _context.Friendships
                .Where(f => f.Status == FriendshipStatus.Accepted &&
                      (f.UserRequesterId == userId || f.UserReceiverId == userId))
                .Select(f => f.UserRequesterId == userId ? f.UserReceiver : f.UserRequester)
                .ToListAsync();
        }

        public async Task<List<Friendship>> GetPendingRequestsAsync(string userId)
        {
            return await _context.Friendships
                .Include(f => f.UserRequester)
                .Where(f => f.UserReceiverId == userId && f.Status == FriendshipStatus.Pending)
                .ToListAsync();
        }
    }
}
