namespace PurchasingSystemDeveloper.Repositories
{
    public interface ISessionService
    {
        bool IsSessionActive(string userId);
        void CreateSession(string userId, string sessionId, DateTime expiration, List<string> roles = null);
        void DeleteSession(string userId);
        void InvalidateAllSessions(string userId);
        List<string> GetRoles(string userId);
        void SaveRoles(string userId, List<string> roles, DateTime expiration);
    }
}
