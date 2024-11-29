namespace PurchasingSystemDeveloper.Repositories
{
    public class UrlMappingService
    {
        // In-memory mapping for demo purposes (use a distributed cache or database in production)
        public Dictionary<string, string> InMemoryMapping { get; } = new();
    }
}
