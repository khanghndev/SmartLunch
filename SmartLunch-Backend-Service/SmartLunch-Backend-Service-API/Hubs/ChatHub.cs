using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Concurrent;

using SmartLunch.Backend.Service.Domain.Time;

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
        // debateId -> (userId -> 1)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> ActiveUsersInDebates = new();
        private static readonly ConcurrentDictionary<string, DateTime> UserLastActivity = new();
        private static readonly ConcurrentDictionary<string, string> UserConnectionMap = new(); // userId -> connectionId
        private static readonly ConcurrentDictionary<string, DateTime> TypingUsers = new(); // connectionId -> timestamp

        // Rate limiting: track message counts per user per debate
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> MessageRateLimits = new();

        public ChatHub(
            ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        private string? GetUserId()
        {
            return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.UserIdentifier;
        }

        public override Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                UserConnectionMap[userId] = Context.ConnectionId;
                UserLastActivity[userId] = VietnamTime.Now;
            }

            _logger.LogInformation("ChatHub connected. ConnectionId={ConnectionId}, UserId={UserId}", Context.ConnectionId, userId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                // remove mapping if it matches this connection
                if (UserConnectionMap.TryGetValue(userId, out var mappedConnId) && mappedConnId == Context.ConnectionId)
                {
                    UserConnectionMap.TryRemove(userId, out _);
                }

                UserLastActivity[userId] = VietnamTime.Now;
            }

            TypingUsers.TryRemove(Context.ConnectionId, out _);
            _logger.LogInformation("ChatHub disconnected. ConnectionId={ConnectionId}, UserId={UserId}", Context.ConnectionId, userId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinDebate(string debateId)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new HubException("Missing user id claim.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, debateId);

            var users = ActiveUsersInDebates.GetOrAdd(debateId, _ => new ConcurrentDictionary<string, byte>());
            users[userId] = 1;
            UserLastActivity[userId] = VietnamTime.Now;

            await Clients.Group(debateId).SendAsync("UserJoined", new
            {
                DebateId = debateId,
                UserId = userId
            });
        }

        public async Task LeaveDebate(string debateId)
        {
            var userId = GetUserId();
            if (!string.IsNullOrWhiteSpace(userId) &&
                ActiveUsersInDebates.TryGetValue(debateId, out var users))
            {
                users.TryRemove(userId, out _);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, debateId);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Clients.Group(debateId).SendAsync("UserLeft", new
                {
                    DebateId = debateId,
                    UserId = userId
                });
            }
        }

        public async Task SendMessage(string debateId, string message)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new HubException("Missing user id claim.");
            }

            if (string.IsNullOrWhiteSpace(debateId))
            {
                throw new HubException("Missing debate id.");
            }

            message = (message ?? string.Empty).Trim();
            if (message.Length == 0)
            {
                return;
            }

            // Simple rate limit: max 20 msgs / 10s per (debateId,userId)
            var now = VietnamTime.Now;
            var rateKey = $"{debateId}:{userId}";
            var q = MessageRateLimits.GetOrAdd(rateKey, _ => new ConcurrentQueue<DateTime>());
            q.Enqueue(now);
            while (q.TryPeek(out var t) && (now - t).TotalSeconds > 10)
            {
                q.TryDequeue(out _);
            }
            if (q.Count > 20)
            {
                throw new HubException("Rate limit exceeded.");
            }

            UserLastActivity[userId] = now;

            await Clients.Group(debateId).SendAsync("ReceiveMessage", new
            {
                DebateId = debateId,
                FromUserId = userId,
                Message = message,
                SentAtUtc = now
            });
        }

        public async Task Typing(string debateId, bool isTyping)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new HubException("Missing user id claim.");
            }

            if (isTyping)
            {
                TypingUsers[Context.ConnectionId] = VietnamTime.Now;
            }
            else
            {
                TypingUsers.TryRemove(Context.ConnectionId, out _);
            }

            await Clients.OthersInGroup(debateId).SendAsync("UserTyping", new
            {
                DebateId = debateId,
                UserId = userId,
                IsTyping = isTyping
            });
        }
    }
}
