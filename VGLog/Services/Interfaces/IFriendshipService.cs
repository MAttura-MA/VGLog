using VGLog.Models;
using VGLog.Models.Enums;

namespace VGLog.Services.Interfaces
{
    public interface IFriendshipService
    {
        Task SendRequestAsync(string requesterId, string receiverId);
        Task AcceptRequestAsync(string currentUserId, string requesterId);
        Task DeclineRequestAsync(string currentUserId, string requesterId);
        Task RemoveFriendAsync(string userId, string friendId);
        Task<FriendshipStatus?> GetStatusAsync(string userA, string userB);
        Task<List<ApplicationUser>> GetFriendsAsync(string userId);
        Task<List<Friendship>> GetPendingRequestsAsync(string userId);
        Task<Dictionary<string, FriendshipStatus?>> GetStatusesAsync(string currentUserId, List<string> otherUserIds);

    }
}
