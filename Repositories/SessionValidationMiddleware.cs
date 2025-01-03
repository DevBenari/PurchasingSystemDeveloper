using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PurchasingSystemDeveloper.Models;
using System.Security.Claims;

namespace PurchasingSystemDeveloper.Repositories
{
    //public class SessionValidationMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ISessionService _sessionService;
    //    private readonly UserManager<ApplicationUser> _userManager;

    //    public SessionValidationMiddleware(RequestDelegate next, UserManager<ApplicationUser> userManager, ISessionService sessionService)
    //    {
    //        _next = next;
    //        _sessionService = sessionService;
    //        _userManager = userManager;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        if (context.User.Identity.IsAuthenticated)
    //        {
    //            var userId = context.Session.GetString("UserId");

    //            if (!string.IsNullOrEmpty(userId))
    //            {
    //                var user = await _userManager.FindByIdAsync(userId);

    //                if (user != null)
    //                {
    //                    // Perbarui status online berdasarkan aktivitas terbaru
    //                    user.IsOnline = false;
    //                    user.LastActivityTime = DateTime.UtcNow;
    //                    await _userManager.UpdateAsync(user);
    //                }

    //                if (!_sessionService.IsSessionActive(userId))
    //                {
    //                    context.Session.Clear();
    //                    context.Response.Redirect("/Account/Login");
    //                    return;
    //                }

    //                var roles = _sessionService.GetRoles(userId);
    //                if (roles == null || roles.Count == 0)
    //                {
    //                    context.Session.Clear();
    //                    context.Response.Redirect("/Account/Login");
    //                    return;
    //                }
    //            }
    //        }

    //        await _next(context);
    //    }
    //}
}
