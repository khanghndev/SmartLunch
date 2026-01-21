using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Concurrent;

namespace SmartLunch.Backend.Service.API.Hubs
{
    /// <summary>
    /// ChatHub is a SignalR hub that allows for real-time communication between users.
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        // Thread-safe collections for tracking active users (consider Redis for production scaling)
        private static readonly ConcurrentDictionary<string, HashSet<string>> ActiveUsersInDebates = new();
        private static readonly ConcurrentDictionary<string, DateTime> UserLastActivity = new();
        private static readonly ConcurrentDictionary<string, string> UserConnectionMap = new(); // userId -> connectionId
        private static readonly ConcurrentDictionary<string, DateTime> TypingUsers = new(); // connectionId -> timestamp

        // Rate limiting: track message counts per user per debate
        private static readonly ConcurrentDictionary<string, List<DateTime>> MessageRateLimits = new();

        public ChatHub(
            ILogger<ChatHub> logger)
        {
            _logger = logger;
        }
    }
}
