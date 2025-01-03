using Microsoft.AspNetCore.Identity;
using PurchasingSystemDeveloper.Models;

namespace PurchasingSystemDeveloper.Repositories
{
    public class CleanInactiveUsersService : BackgroundService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CleanInactiveUsersService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanInactiveUsers();
                }
                catch (Exception ex)
                {
                    // Log error jika ada masalah
                    Console.WriteLine($"Error cleaning inactive users: {ex.Message}");
                }

                // Jalankan setiap 10 menit
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private void CleanInactiveUsers()
        {
            var inactiveThreshold = DateTime.UtcNow.AddMinutes(-10);
            var inactiveUsers = _userManager.Users
                .Where(u => u.IsOnline && u.LastActivityTime < inactiveThreshold)
                .ToList();

            foreach (var user in inactiveUsers)
            {
                user.IsOnline = false;
                user.LastActivityTime = null;

                // Simpan perubahan secara sinkron
                _userManager.UpdateAsync(user).Wait();
            }

            Console.WriteLine("Inactive users cleaned successfully.");
        }
    }
}
