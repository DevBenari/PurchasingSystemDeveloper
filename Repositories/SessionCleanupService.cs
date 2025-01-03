using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PurchasingSystemDeveloper.Models;

namespace PurchasingSystemDeveloper.Repositories
{
    public class SessionCleanupService
    {
        private readonly RequestDelegate _next;
        private readonly TimeSpan idleTimeout = TimeSpan.FromMinutes(15);

        public SessionCleanupService(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.GetUserAsync(context.User);

                if (user != null)
                {
                    var now = DateTime.UtcNow;
                    var lastActivity = user.LastActivityTime ?? now;
                    if (now - lastActivity > idleTimeout)
                    {
                        // Waktu idle melebihi batas -> sesi kadaluarsa
                        user.IsOnline = false;
                        await userManager.UpdateAsync(user);

                        // Sign out user
                        await context.SignOutAsync("CookieAuth");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                    else
                    {
                        // Perbarui waktu aktivitas
                        user.LastActivityTime = now;
                        await userManager.UpdateAsync(user);
                    }
                }
            }

            await _next(context);
        }
    }
}
