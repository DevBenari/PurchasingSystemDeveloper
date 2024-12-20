using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using PurchasingSystemDeveloper.Repositories;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using PurchasingSystemDeveloper.Controllers;
using System;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[controller]/[action]")]
    public class GroupRoleController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IGroupRoleRepository _groupRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHubContext<ChatHub> _hubContext;

        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;

        public GroupRoleController(ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IGroupRoleRepository groupRoleRepository,
            IRoleRepository roleRepository,
            IHubContext<ChatHub> hubContext,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService
        )
        {
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
            _userActiveRepository = userActiveRepository;
            _groupRoleRepository = groupRoleRepository;
            _roleRepository = roleRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
        }

        public IActionResult RedirectToIndex()
        {
            try
            {
                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:MasterData/GroupRole/Index";
                string encryptedPath = _protector.Protect(originalPath);

                // Hash GUID-like code (SHA256 truncated to 36 characters)
                string guidLikeCode = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(encryptedPath)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Substring(0, 36);

                // Simpan mapping GUID-like code ke encryptedPath di penyimpanan sementara (misalnya, cache)
                _urlMappingService.InMemoryMapping[guidLikeCode] = encryptedPath;

                return Redirect("/" + guidLikeCode);
            }
            catch
            {
                // Jika enkripsi gagal, kembalikan view
                return View();
            }            
        }

        public IActionResult Index()
        {
            ViewBag.Active = "MasterData";
            return View(); // Kirim data role ke view
        }      

        [HttpGet]
        public async Task<IActionResult> CreateRole()
        {
            ViewBag.Active = "MasterData";
            var roles = await _roleManager.Roles
                      .Select(r => new
                      {
                          r.Id,
                          r.Name,
                          r.NormalizedName
                      })
                      .ToListAsync();
            // Kirim data role langsung ke view
            return View(roles);
        }        

        [HttpPost]
        public async Task<IActionResult> CreateRole(GroupRoleViewModel vm)
        {
            ViewBag.Active = "MasterData";
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var getUser = _userActiveRepository.GetAllUserLogin()
                .FirstOrDefault(u => u.UserName == vm.DepartemenId);

            if (getUser == null)
            {
                TempData["WarningMessage"] = "Sorry, please select a user first !!!";
                return RedirectToAction("RedirectToIndex"); // atau aksi lain sesuai kebutuhan                
            }
            else
            {
                var departemenId = getUser.Id;
                var roleIds = vm.RoleId;
                // Hapus semua user roles terkait
                _groupRoleRepository.DeleteByDepartmentId(departemenId);
                var userRoles = _applicationDbContext.UserRoles
                                                      .Where(ur => ur.UserId == departemenId)
                                                      .ToList();
                if (userRoles.Any())
                {
                    _applicationDbContext.UserRoles.RemoveRange(userRoles);
                    _applicationDbContext.SaveChanges();  // Simpan perubahan ke database
                }
                // End Hapus 

                if (ModelState.IsValid)
                {
                    // Simpan ID Peran
                    foreach (var roleId in roleIds)
                    {
                        var groupRole = new GroupRole
                        {
                            DepartemenId = departemenId,
                            RoleId = roleId, // Gunakan roleId dari loop
                            CreateDateTime = DateTime.Now,
                            CreateBy = new Guid(getUser.Id)
                        };

                        _groupRoleRepository.Tambah(groupRole);


                        var role = await _roleManager.FindByIdAsync(roleId.ToString());
                        if (role != null)
                        {
                            // Membuat entri baru untuk AspNetUserRoles
                            var userRole = new IdentityUserRole<string>
                            {
                                UserId = getUser.Id,  // Menggunakan UserId dari pengguna
                                RoleId = roleId.ToString()  // Menggunakan RoleId sebagai string
                            };

                            // Menambahkan entri ke tabel AspNetUserRoles
                            _applicationDbContext.UserRoles.Add(userRole);
                            await _applicationDbContext.SaveChangesAsync();  // Menyimpan perubahan ke database
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Role dengan ID {roleId} tidak ditemukan.");
                            return View();
                        }
                    }
                }

                TempData["SuccessMessage"] = "Role successfully assigned to user";
                return RedirectToAction("RedirectToIndex"); // atau aksi lain sesuai kebutuhan
            }                        
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleNavbar()
        {
            ViewBag.Active = "MasterData";
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            foreach (var controllerType in controllers)
            {
                var controllerName = controllerType.Name.Replace("Controller", ""); // Nama controller tanpa "Controller"
                var controllerActions = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(method => method.IsPublic && !method.IsSpecialName && method.DeclaringType == controllerType)
                    .Select(method => method.Name)
                    .ToList();

                foreach (var action in controllerActions)
                {
                    string roleName = action;

                    // Jika aksi adalah "Index", tambahkan nama controller ke role
                    if (action == "Index")
                    {
                        roleName = $"Index{controllerName}";  // Misalnya, "AdminIndex"
                    }

                    // Periksa apakah role sudah ada
                    var roleExists = await _roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        IdentityRole role = new IdentityRole
                        {
                            Name = roleName,  // Nama asli role (misalnya, "AdminIndex")
                            ConcurrencyStamp = controllerName
                        };

                        var result = await _roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                Console.WriteLine($"Error creating role {roleName}: {error.Description}");
                            }
                        }
                    }
                }
            }

            await _hubContext.Clients.All.SendAsync("UpdateDataCount", '0');
            return Json(new { success = true, message = "Role untuk semua controller berhasil dibuat." });
        }

        public void DeleteUserRoleByDepartmentId(string departemenId)
        {
            // Cari semua user roles yang terkait dengan departemenId (UserId)
            var userRoles = _applicationDbContext.UserRoles
                                                  .Where(ur => ur.UserId == departemenId)
                                                  .ToList();

            if (userRoles.Any())
            {
                // Hapus semua user roles terkait
                _applicationDbContext.UserRoles.RemoveRange(userRoles);
                _applicationDbContext.SaveChanges();  // Simpan perubahan ke database
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoadRoles(string Email)
        {
            ViewBag.Active = "MasterData";
            if (!string.IsNullOrEmpty(Email))
            {
                var userId = _userActiveRepository.GetAllUserLogin()
                    .FirstOrDefault(u => u.UserName == Email);
                // Mengambil bukan DepartemenId
                var roleIdsNotForDep = _applicationDbContext.GroupRoles
                .Where(gr => gr.DepartemenId == userId.Id)
                .Select(gr => gr.RoleId)
                .ToList();

                var rolesNotForDep = _roleManager.Roles
                    .Where(role => roleIdsNotForDep.Contains(role.Id))
                    .OrderBy(role => role.ConcurrencyStamp)
                    .ToList();

                // Filter roles yang hanya ada di roleIds dan bukan di roleIdsNotForDep
                var allRoles = _roleManager.Roles.ToList();
                var rolesForDep = allRoles
                    .Where(role => !roleIdsNotForDep.Contains(role.Id))
                    .OrderBy(role => role.ConcurrencyStamp)
                    .ToList();

                var result = new
                {
                    RolesForDepartment = rolesForDep,
                    RolesNotForDepartment = rolesNotForDep
                };

                return Json(result);
            }
            else
            {
                var roles = _roleManager.Roles
                    .OrderBy(role => role.ConcurrencyStamp)
                    .ToList();
                return Json(new
                {
                    RolesForDepartment = roles
                });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult LoadUser()
        {
            var user = _userActiveRepository.GetAllUser().ToList();
            return Json(user);
        }

    }
}
