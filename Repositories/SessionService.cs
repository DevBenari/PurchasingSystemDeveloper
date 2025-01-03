using Microsoft.Extensions.Caching.Memory;

namespace PurchasingSystemDeveloper.Repositories
{
    public class SessionService : ISessionService
    {
        private readonly IMemoryCache _cache;

        public SessionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool IsSessionActive(string userId)
        {
            return _cache.TryGetValue($"Session_{userId}", out _);
        }

        public void CreateSession(string userId, string sessionId, DateTime expiration, List<string> roles = null)
        {
            _cache.Set($"Session_{userId}", sessionId, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            });

            if (roles != null)
            {
                SaveRoles(userId, roles, expiration);
            }
        }

        public void DeleteSession(string userId)
        {
            _cache.Remove($"Session_{userId}");
            _cache.Remove($"Roles_{userId}");
        }

        public void InvalidateAllSessions(string userId)
        {
            DeleteSession(userId);
        }

        public List<string> GetRoles(string userId)
        {
            if (_cache.TryGetValue($"Roles_{userId}", out List<string> roles))
            {
                return roles;
            }
            return new List<string>();
        }

        public void SaveRoles(string userId, List<string> roles, DateTime expiration)
        {
            _cache.Set($"Roles_{userId}", roles, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            });
        }
    }
}
