using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IRoleRepository roleRepository,
            IGroupRoleRepository groupRoleRepository,
            ILogger<AccountController> logger)
        {
            _applicationDbContext = applicationDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _userActiveRepository = userActiveRepository;
            _logger = logger;
            _roleRepository = roleRepository;
            _groupRoleRepository = groupRoleRepository;
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
            HttpContext.Session.Clear();

            // Hapus authentication cookies jika ada
            Response.Cookies.Delete(".AspNetCore.Identity.Application");

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
            var response = new LoginViewModel();
            return View(response);
            //return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt. ");
                    TempData["WarningMessage"] = "Sorry, Username & Password Not Registered !";
                    return View(model);
                }
                else if (user.IsActive == true) 
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                        new Claim("amr", "pwd"),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        //Membuat sesi pengguna
                        HttpContext.Session.SetString("Username", user.Email);

                        //Membuat cookies
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = false, // Set ke true jika ingin session bertahan setelah browser ditutup
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authProperties);

                        var roles = await _signInManager.UserManager.GetRolesAsync(user);

                        if (roles.Any())
                        {
                            var roleClaim = string.Join(",", roles);
                            claims.Add(new Claim("Roles", roleClaim));
                        }

                        await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);
                        
                        // menyimpan data user yang sedang login berdasarkan NamaUser dan KodeUser
                        HttpContext.Session.SetString("FullName", user.NamaUser);
                        HttpContext.Session.SetString("KodeUser", user.KodeUser);

                        // Role
                        List<string> roleNames;

                        if (model.Email == "superadmin@admin.com")
                        {
                            roleNames = Assembly.GetExecutingAssembly().GetTypes()
                                                .Where(type => typeof(Controller).IsAssignableFrom(type))
                                                .Select(type => type.Name.Replace("Controller", "")) // Menghapus akhiran "Controller"
                                                .ToList();
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

                                roleNames = (from role in _roleRepository.GetRoles()
                                             join userRole in _groupRoleRepository.GetAllGroupRole()
                                             on role.Id equals userRole.RoleId
                                             where userRole.DepartemenId == userId
                                             select role.ConcurrencyStamp).Distinct().ToList();
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
                        return RedirectToAction("Index", "Home");
                    }
                    
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, model.RememberMe });
                    }

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
                else
                {
                    TempData["UserActiveMessage"] = "Sorry, your account is not active !";
                    return View(model);
                }             
            }
            return View(model);
        }
        

        public async Task<IActionResult> Logout()
        {
            // Hapus semua session
            HttpContext.Session.Clear();

            // Hapus cookie UserName
            Response.Cookies.Delete("Username");

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var user = await _signInManager.UserManager.FindByNameAsync(getUser.Email);

            if (user != null)
            {
                user.IsOnline = false;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
