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

        public GroupRoleController(ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IGroupRoleRepository groupRoleRepository,
            IRoleRepository roleRepository,
            IHubContext<ChatHub> hubContext
        )
        {
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
            _userActiveRepository = userActiveRepository;
            _groupRoleRepository = groupRoleRepository;
            _roleRepository = roleRepository;
        }

        public IActionResult Index()
        {
            return View(); // Kirim data role ke view
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRole()
        {
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

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateRole(GroupRoleViewModel vm)
        //{
        //    var dateNow = DateTimeOffset.Now;
        //    var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

        //    // Ambil pengguna berdasarkan `UserId`
        //    var getUser = _userActiveRepository.GetAllUserLogin()
        //        .FirstOrDefault(u => u.UserName == vm.DepartemenId);

        //    if (ModelState.IsValid)
        //    {
        //        var roleIds = vm.RoleId; // RoleIds adalah List<string> yang sesuai
        //        var userId = getUser.Id;

        //        // Menghapus peran pengguna yang ada di `AspNetUserRoles`
        //        var userRoles = await _userManager.GetRolesAsync(getUser);
        //        foreach (var role in userRoles)
        //        {
        //            await _userManager.RemoveFromRoleAsync(getUser, role);
        //        }

        //        // Menambahkan peran baru untuk pengguna
        //        foreach (var roleId in roleIds)
        //        {
        //            var role = await _roleManager.FindByIdAsync(roleId);
        //            if (role != null)
        //            {
        //                var result = await _userManager.AddToRoleAsync(getUser, role.Name);
        //                if (!result.Succeeded)
        //                {
        //                    foreach (var error in result.Errors)
        //                    {
        //                        ModelState.AddModelError("", error.Description);
        //                    }
        //                }
        //            }
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            return RedirectToAction("Index"); // Kembali ke halaman `Index` atau halaman lain sesuai kebutuhan
        //        }
        //    }

        //    return View(vm); // Kembali ke view jika ada error
        //}
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRole(GroupRoleViewModel vm)
        {
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            var getUser = _userActiveRepository.GetAllUserLogin()
                .FirstOrDefault(u => u.UserName == vm.DepartemenId);

            if (ModelState.IsValid)
            {
                var departemenId = getUser.Id;
                var roleIds = vm.RoleId; // Pastikan RoleIds adalah List<string> atau koleksi yang sesuai

                _groupRoleRepository.DeleteByDepartmentId(departemenId);
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
                }

                // Role
                //var desiredDepartemenId = _userActiveRepository.GetAllUser()
                //    .FirstOrDefault(u => u.Email == User.Identity.Name)?.DepartmentId;

                //var roleNames = (from role in _roleRepository.GetRoles()
                //                 join groupRole in _groupRoleRepository.GetAllGroupRole()
                //                 on role.Id equals groupRole.RoleId
                //                 where groupRole.DepartemenId == desiredDepartemenId.ToString()
                //                 select role.Name).ToList();

                //HttpContext.Session.SetString("ListRole", string.Join(",", roleNames));
                // End Role
            }
            return RedirectToAction("Index"); // atau aksi lain sesuai kebutuhan

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRoleNavbar(string roleName, string normalized)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return Json(new { success = false, message = "Nama role diperlukan." });
            }
            else if (string.IsNullOrEmpty(normalized))
            {
                return Json(new { success = false, message = "Nama normalized diperlukan." });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = roleName, // Set normalized name dengan benar
                    ConcurrencyStamp = normalized.ToUpper() // Atau gunakan logika Anda untuk ini
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    await _hubContext.Clients.All.SendAsync("UpdateDataCount", '0');
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Gagal membuat role." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Role sudah ada." });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoadRoles(string Email)
        {

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
                    .ToList();

                // Filter roles yang hanya ada di roleIds dan bukan di roleIdsNotForDep
                var allRoles = _roleManager.Roles.ToList();
                var rolesForDep = allRoles
                    .Where(role => !roleIdsNotForDep.Contains(role.Id))
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
                var roles = _roleManager.Roles.ToList();
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
