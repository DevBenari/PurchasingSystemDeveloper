using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.ViewModels;
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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            ILogger<AccountController> logger)
        {
            _applicationDbContext = applicationDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _userActiveRepository = userActiveRepository;
            _logger = logger;
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
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                    {
                        new Claim("amr", "pwd"),
                    };

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
                        
                        user.IsOnline = true;

                        await _userManager.UpdateAsync(user);

                        _logger.LogInformation("User logged in.");
                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, model.RememberMe });
                    }

                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        // HttpContext.session.Clear untuk menghapus session data pengguna tidak lagi tersimpan
                        HttpContext.Session.Clear();
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        TempData["WarningMessage"] = "Sorry, Wrong Password !";
                        return View(model);
                    }
                }
                else
                {
                    TempData["UserActiveMessage"] = "Sorry, your user is not active !";
                    return View(model);
                }
                
                //if (result.Succeeded)
                //{
                //    _logger.LogInformation("User Loggin in.");
                //    return RedirectToAction("Index", "Home");
                //}                
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if(token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmPasswordConfiguration");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }



    }
}
