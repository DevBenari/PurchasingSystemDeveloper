using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using PurchasingSystemDeveloper.ViewModels;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PurchasingSystemDeveloper.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IGroupRoleRepository _groupRoleRepository;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IRoleRepository roleRepository,
            IGroupRoleRepository groupRoleRepository,
            ILogger<AccountController> logger,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService)
        {
            _applicationDbContext = applicationDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _userActiveRepository = userActiveRepository;
            _logger = logger;
            _roleRepository = roleRepository;
            _groupRoleRepository = groupRoleRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public IActionResult Index()
        {            
            return View();
        }


        [HttpPost]
        [Route("accountController/ExtendSession")]
        public IActionResult ExtendSession()
        {
            // Memperpanjang session dengan memperbarui waktu aktivitas terakhir
            HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString());

            return Ok();
        }

        [HttpPost]
        [Route("accountController/EndSession")]
        public async Task<IActionResult> EndSession()
        {
            //HttpContext.Session.Clear();

            //// Hapus authentication cookies jika ada
            //Response.Cookies.Delete(".AspNetCore.Identity.Application");

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var user = await _signInManager.UserManager.FindByNameAsync(getUser.Email);

            if (user != null)
            {
                user.IsOnline = false;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();

            return Ok();
        }        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("RedirectToIndex", "Home");
            }
            else
            {
                var response = new LoginViewModel();
                return View(response);
            }            
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model/*, string returnUrl = null*/)
        {
            //returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("RedirectToIndex", "Home");
                }
                else {
                    var user = await _signInManager.UserManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt. ");
                        TempData["WarningMessage"] = "Sorry, Username And Password Not Registered !";
                        return View(model);
                    }
                    else if (user.IsActive == true && user.IsOnline == false)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
                        if (result.Succeeded)
                        {
                            //Membuat sesi pengguna
                            HttpContext.Session.SetString("username", user.Email);

                            // Set cookie autentikasi
                            var claims = new[] { new Claim(ClaimTypes.Name, user.Email) };
                            var identity = new ClaimsIdentity(claims, "CookieAuth");
                            var principal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync("CookieAuth", principal);

                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = false, // Set ke true jika ingin session bertahan setelah browser ditutup
                            };

                            //var roles = await _signInManager.UserManager.GetRolesAsync(user);

                            //if (roles.Any())
                            //{
                            //    var roleClaim = string.Join(",", roles);
                            //    claims.Add(new Claim("Roles", roleClaim));
                            //}

                            await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);

                            // Role
                            List<string> roleNames; // Deklarasikan di luar

                            if (model.Email == "superadmin@admin.com")
                            {
                                roleNames = _roleRepository.GetRoles().Select(role => role.Name).ToList();
                            }
                            else
                            {
                                var userId = _userActiveRepository.GetAllUserLogin()
                                    .FirstOrDefault(u => u.UserName == model.Email)?.Id;

                                if (userId != null) // Pastikan userId tidak null
                                {
                                    roleNames = (from role in _roleRepository.GetRoles()
                                                 join userRole in _groupRoleRepository.GetAllGroupRole()
                                                 on role.Id equals userRole.RoleId
                                                 where userRole.DepartemenId == userId 
                                                 select role.Name).ToList();
                                    // ambil juga judul modul
                                    roleNames = (from role in _roleRepository.GetRoles()
                                                 join userRole in _groupRoleRepository.GetAllGroupRole()
                                                 on role.Id equals userRole.RoleId
                                                 where userRole.DepartemenId == userId
                                                 select role.ConcurrencyStamp)
                                                 .Distinct().ToList();


                                }
                                else
                                {
                                    roleNames = new List<string>(); // Jika userId tidak ditemukan, set roleNames ke list kosong
                                }
                            }

                            // Menyimpan daftar roleNames ke dalam session
                            HttpContext.Session.SetString("ListRole", string.Join(",", roleNames));

                            user.IsOnline = true;

                            await _userManager.UpdateAsync(user);

                            _logger.LogInformation("User logged in.");
                            return RedirectToAction("RedirectToIndex", "Home");
                        }

                        //if (result.RequiresTwoFactor)
                        //{
                        //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, model.RememberMe });
                        //}

                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            // HttpContext.session.Clear untuk menghapus session data pengguna tidak lagi tersimpan
                            HttpContext.Session.Clear();

                            // Hitung waktu yang tersisa
                            var lockTime = await _userManager.GetLockoutEndDateAsync(user);
                            var timeRemaining = lockTime.Value - DateTimeOffset.UtcNow;

                            TempData["UserLockOut"] = "Sorry, your account is locked in " + timeRemaining.Minutes + " minutes " + timeRemaining.Seconds + " seconds";
                            return View(model);
                        }

                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        TempData["WarningMessage"] = "Sorry, Wrong Password !";
                        //return View(model);
                    }
                    else if (user.IsActive == true && user.IsOnline == true)
                    {
                        TempData["UserOnlineMessage"] = "Sorry, your account is online, has been logged out, please sign back in !";

                        user.IsOnline = false;
                        await _userManager.UpdateAsync(user);

                        return View(model);
                    }
                    else
                    {
                        TempData["UserActiveMessage"] = "Sorry, your account is not active !";
                        return View(model);
                    }
                }                            
            }
            return View();
        }
                
        public async Task<IActionResult> Logout()
        {
            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var user = await _signInManager.UserManager.FindByNameAsync(getUser.Email);          

            if (user != null)
            {
                user.IsOnline = false;
                await _userManager.UpdateAsync(user);
            }

            // Hapus session dan sign out cookie
            HttpContext.Session.Remove("username");
            await HttpContext.SignOutAsync("CookieAuth");

            await _signInManager.SignOutAsync();
            return RedirectToAction("RedirectToIndex", "Home");
        }
    }
}
