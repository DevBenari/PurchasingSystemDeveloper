using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.MasterData.ViewModels;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Order.Repositories;
using PurchasingSystemDeveloper.Areas.Transaction.Repositories;
using PurchasingSystemDeveloper.Areas.Warehouse.Repositories;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Hubs;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;
using PurchasingSystemDeveloper.ViewModels;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Web.Helpers;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Web.WebPages;

namespace PurchasingSystemDeveloper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IProductRepository _productRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IQtyDifferenceRepository _qtyDifferenceRepository;
        private readonly IApprovalQtyDifferenceRepository _approvalQtyDifferenceRepository;
        private readonly IUnitRequestRepository _unitRequestRepository;
        private readonly IApprovalUnitRequestRepository _approvalUnitRequestRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        
        private readonly IDataProtector _protector;
        private readonly UrlMappingService _urlMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IHubContext<ChatHub> hubContext,
            IPurchaseRequestRepository purchaseRequestRepository,
            IUserActiveRepository userActiveRepository,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            IProductRepository productRepository,
            IApprovalRepository approvalRepository,
            IQtyDifferenceRepository qtyDifferenceRepository,
            IApprovalQtyDifferenceRepository approvalQtyDifferenceRepository,
            IUnitRequestRepository unitRequestRepository,
            IApprovalUnitRequestRepository approvalUnitRequestRepository,
            IPurchaseOrderRepository purchaseOrderRepository,

            IDataProtectionProvider provider,
            UrlMappingService urlMappingService,
            IHostingEnvironment hostingEnvironment
        )
        {
            _logger = logger;
            _applicationDbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
            _userActiveRepository = userActiveRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _purchaseRequestRepository = purchaseRequestRepository;
            _productRepository = productRepository;
            _approvalRepository = approvalRepository;
            _qtyDifferenceRepository = qtyDifferenceRepository;
            _approvalQtyDifferenceRepository = approvalQtyDifferenceRepository;
            _unitRequestRepository = unitRequestRepository;
            _approvalUnitRequestRepository = approvalUnitRequestRepository;
            _purchaseOrderRepository = purchaseOrderRepository;

            _protector = provider.CreateProtector("UrlProtector");
            _urlMappingService = urlMappingService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult RedirectToIndex()
        {
            try
            {
                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Home/Index";
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

        public async Task<IActionResult> Index(string filterOptions = "", string searchTerm = "", DateTimeOffset? startDate = null, DateTimeOffset? endDate = null, int page = 1, int pageSize = 10)
        {
            ViewBag.Active = "Dashboard";
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedFilter = filterOptions;

            // Format tanggal untuk input[type="date"]
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // Format tanggal untuk tampilan (Indonesia)
            ViewBag.StartDateReadable = startDate?.ToString("dd MMMM yyyy");
            ViewBag.EndDateReadable = endDate?.ToString("dd MMMM yyyy");

            // Normalisasi tanggal untuk mengabaikan waktu
            if (startDate.HasValue) startDate = startDate.Value.Date;
            if (endDate.HasValue) endDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Sampai akhir hari

            // Tentukan range tanggal berdasarkan filterOptions
            if (!string.IsNullOrEmpty(filterOptions))
            {
                (startDate, endDate) = GetDateRangeHelper.GetDateRange(filterOptions);
            }

            var checkUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var getUserActive = _userActiveRepository.GetAllUser().Where(c => c.UserActiveCode == checkUserLogin.KodeUser).FirstOrDefault();
            var userLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.IsOnline == true).ToList();
            var user = _userActiveRepository.GetAllUser().Where(u => u.FullName == checkUserLogin.NamaUser).FirstOrDefault();
            var data = await _productRepository
                .GetAllProductPageSize(searchTerm, page, pageSize, startDate, endDate)
                /*.Where(p => p.Stock < p.MinStock).ToList()*/;

            if (user == null && checkUserLogin.Email == "superadmin@admin.com")
            {
                UserActiveViewModel viewModel = new UserActiveViewModel
                {
                    UserActiveCode = checkUserLogin.KodeUser,
                    FullName = checkUserLogin.NamaUser,
                    Email = checkUserLogin.Email
                };

                var dashboard = new Dashboard();
                {
                    dashboard.UserActiveViewModels = viewModel;
                    dashboard.UserOnlines = userLogin;
                    dashboard.Products = data.products;
                }

                return View(dashboard);
            }
            else
            {
                UserActiveViewModel viewModel = new UserActiveViewModel
                {
                    UserActiveId = user.UserActiveId,
                    UserActiveCode = user.UserActiveCode,
                    FullName = user.FullName,
                    IdentityNumber = user.IdentityNumber,
                    DepartmentId = user.DepartmentId,
                    Department = user.Department.DepartmentName,
                    PositionId = user.PositionId,
                    Position = user.Position.PositionName,
                    PlaceOfBirth = user.PlaceOfBirth,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Address = user.Address,
                    Handphone = user.Handphone,
                    Email = user.Email,
                    UserPhotoPath = user.Foto
                };

                var dashboard = new Dashboard();
                {
                    dashboard.UserActiveViewModels = viewModel;
                    dashboard.UserOnlines = userLogin;
                    dashboard.Products = data.products;
                }

                var userOnline = _userActiveRepository.GetAllUserLogin().Where(u => u.IsOnline == true).GroupBy(u => u.Id).Select(y => new
                {
                    Id = y.Key,
                    CountOfUsers = y.Count()
                }).ToList();
                ViewBag.UserOnline = userOnline.Count;

                var countPurchaseRequest = _applicationDbContext.PurchaseRequests.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.PurchaseRequestId).Select(y => new
                {
                    PurchaseRequestId = y.Key,
                    CountOfPurchaseRequests = y.Count()
                }).ToList();
                ViewBag.CountPurchaseRequest = countPurchaseRequest.Count;

                var countPurchaseOrder = _applicationDbContext.PurchaseOrders.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.PurchaseOrderId).Select(y => new
                {
                    PurchaseOrderId = y.Key,
                    CountOfPurchaseOrders = y.Count()
                }).ToList();
                ViewBag.CountPurchaseOrder = countPurchaseOrder.Count;

                var countApproval = _applicationDbContext.Approvals.Where(u => u.UserApproveId == getUserActive.UserActiveId && u.Status == "Approve").ToList();
                ViewBag.CountApproval = countApproval.Count;

                var countUnitRequest = _applicationDbContext.UnitRequests.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.UnitRequestId).Select(y => new
                {
                    UnitRequestId = y.Key,
                    CountOfUnitRequests = y.Count()
                }).ToList();
                ViewBag.CountUnitRequest = countUnitRequest.Count;

                var countReceiveOrder = _applicationDbContext.ReceiveOrders.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.ReceiveOrderId).Select(y => new
                {
                    ReceiveOrderId = y.Key,
                    CountOfReceiveOrders = y.Count()
                }).ToList();
                ViewBag.CountReceiveOrder = countReceiveOrder.Count;

                var countApprovalUnitRequest = _applicationDbContext.ApprovalUnitRequests.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.ApprovalUnitRequestId).Select(y => new
                {
                    ApprovalRequestId = y.Key,
                    CountOfApprovalUnitRequests = y.Count()
                }).ToList();
                ViewBag.CountApprovalUnitRequest = countApprovalUnitRequest.Count;

                var countWarehouseTransfer = _applicationDbContext.WarehouseTransfers.Where(u => u.CreateBy == new Guid(checkUserLogin.Id)).GroupBy(u => u.WarehouseTransferId).Select(y => new
                {
                    WarehouseTransferId = y.Key,
                    CountOfWarehouseTransfers = y.Count()
                }).ToList();
                ViewBag.CountWarehouseTransfer = countWarehouseTransfer.Count;                

                return View(dashboard);
            }
        }

        public IActionResult RedirectToProfile()
        {
            try
            {
                // Bangun originalPath dengan format tanggal ISO 8601
                string originalPath = $"Page:Home/MyProfile";
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

        [HttpGet]
        public IActionResult MyProfile()
        {
            ViewBag.Active = "Dashboard";
            var countUser = _applicationDbContext.UserActives.GroupBy(u => u.UserActiveId).Select(y => new
            {
                UserActiveId = y.Key,
                CountOfUsers = y.Count()
            }).ToList();
            ViewBag.CountUser = countUser.Count;

            var countSupplier = _applicationDbContext.Suppliers.GroupBy(u => u.SupplierId).Select(y => new
            {
                SupplierId = y.Key,
                CountOfSuppliers = y.Count()
            }).ToList();
            ViewBag.CountSupplier = countSupplier.Count;

            var countProduct = _applicationDbContext.Products.GroupBy(u => u.ProductId).Select(y => new
            {
                ProductId = y.Key,
                CountOfProducts = y.Count()
            }).ToList();
            ViewBag.CountProduct = countProduct.Count;

            var countPurchaseRequest = _applicationDbContext.PurchaseRequests.GroupBy(u => u.PurchaseRequestId).Select(y => new
            {
                PurchaseRequestId = y.Key,
                CountOfPurchaseRequests = y.Count()
            }).ToList();
            ViewBag.CountPurchaseRequest = countPurchaseRequest.Count;

            var countPurchaseOrder = _applicationDbContext.PurchaseOrders.GroupBy(u => u.PurchaseOrderId).Select(y => new
            {
                PurchaseOrderId = y.Key,
                CountOfPurchaseOrders = y.Count()
            }).ToList();
            ViewBag.CountPurchaseOrder = countPurchaseOrder.Count;

            var countReceiveOrder = _applicationDbContext.ReceiveOrders.GroupBy(u => u.ReceiveOrderId).Select(y => new
            {
                ReceiveOrderId = y.Key,
                CountOfReceiveOrders = y.Count()
            }).ToList();
            ViewBag.CountReceiveOrder = countReceiveOrder.Count;

            var checkUserLogin = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var user = _userActiveRepository.GetAllUser().Where(u => u.FullName == checkUserLogin.NamaUser).FirstOrDefault();

            if (user != null)
            {
                UserActiveViewModel viewModel = new UserActiveViewModel
                {
                    UserActiveId = user.UserActiveId,
                    UserActiveCode = user.UserActiveCode,
                    FullName = user.FullName,
                    IdentityNumber = user.IdentityNumber,
                    DepartmentId = user.DepartmentId,
                    Department = user.Department.DepartmentName,
                    PositionId = user.PositionId,
                    Position = user.Position.PositionName,
                    PlaceOfBirth = user.PlaceOfBirth,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Address = user.Address,
                    Handphone = user.Handphone,
                    Email = user.Email,
                    UserPhotoPath = user.Foto,
                    IsActive = user.IsActive
                };
                return View(viewModel);
            }
            else if (user == null && checkUserLogin.NamaUser == "SuperAdmin")
            {
                UserActiveViewModel viewModel = new UserActiveViewModel
                {
                    UserActiveCode = checkUserLogin.KodeUser,
                    FullName = checkUserLogin.NamaUser,
                    Email = checkUserLogin.Email
                };
                return View(viewModel);
            }
            return View();
        }

        [HttpPost]
        public IActionResult MyProfile(UserActiveViewModel model)
        {
            if (model.Foto != null)
            {
                // Proses unggah gambar menggunakan fungsi ProcessUploadFile
                var newPhotoPath = ProcessUploadFile(model);

                // Cari user yang sedang login
                var checkUserLogin = _userActiveRepository.GetAllUserLogin()
                    .FirstOrDefault(u => u.UserName == User.Identity.Name);
                var user = _userActiveRepository.GetAllUser()
                    .FirstOrDefault(u => u.FullName == checkUserLogin.NamaUser);

                if (user != null)
                {
                    // Update path foto di database
                    user.Foto = newPhotoPath;
                    _userActiveRepository.Update(user);
                    _applicationDbContext.SaveChanges();
                    TempData["Message"] = "Profile photo updated successfully!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid image file.";
            }

            // Kembali ke halaman profil setelah upload
            return RedirectToAction("MyProfile");
        }

        [HttpPost]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = new Guid(user.Id);
            var data = _userActiveRepository.GetAllUser().Where(x => x.CreateBy == userId).ToList();

            //// Gunakan userId untuk melakukan sesuatu, misalnya:
            ViewBag.UserId = userId;
            return Json(data);
        }

        public IActionResult GetMonitoringProduct()
        {
            var alldata = _productRepository.GetAllProduct();
            var warning = alldata.Where(product => product.Stock < product.MinStock).Count(); // Data Stock Dibawah Minimal Stock (Minstock) bukan termasuk data stock yang save
            var save = alldata.Where(product => product.Stock >= product.MinStock).Count(); // Data Stock Sama Atau Diatas Minimal Stock (Minstock) termasuk data stock yang aman

            //var warning = _productRepository.GetAllProduct().Where(product => product.Stock <= product.MinStock).Count();
            //var save = _productRepository.GetAllProduct().Where(product => product.Stock >= product.MinStock).Count();
            
            var result = new
            {
                Warning = warning,
                Save = save
            };
            return Json(result);
        }

        public IActionResult GetMonitoringStatus()
        {
            var completed = _purchaseOrderRepository.GetAllPurchaseOrder().Where(po => po.Status.StartsWith("RO")).Count();
            var inorder = _purchaseOrderRepository.GetAllPurchaseOrder().Where(po => po.Status == "In Order").Count();
            var cancelled = _purchaseOrderRepository.GetAllPurchaseOrder().Where(po => po.Status == "Cancelled").Count();
            var result = new
            {
                Completed = completed,
                Inorder = inorder,
                Cancelled = cancelled
            };
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Invalid password reset token");
                return View("Error");
            }

            var model = new ResetPasswordViewModel { NewPassword = password };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)    
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid input.";
                return RedirectToAction("MyProfile");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                return RedirectToAction("MyProfile");
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);  
            }

            if (result.Succeeded)
            {
                TempData["MessageSuccess"] = "Password changed successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("MyProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            TempData["MessageFailed"] = "Failed to change password.";
            return RedirectToAction("MyProfile");
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

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]        
        public IActionResult CountNotifikasi()
        {
            var getUserId = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (getUserId.Email == "superadmin@admin.com")
            {
                return Json(new { success = true });
            } 
            else 
            {
                var getUserActiveId = _userActiveRepository.GetAllUser().Where(u => u.UserActiveCode == getUserId.KodeUser).FirstOrDefault().UserActiveId;
                var getUserActiveCode = _userActiveRepository.GetAllUser().Where(u => u.UserActiveCode == getUserId.KodeUser).FirstOrDefault().UserActiveCode;
                var loggerDataPR = new List<object>();
                var loggerDataQtyDiff = new List<object>();
                var loggerDataUnitReq = new List<object>();

                var DataPR = _purchaseRequestRepository.GetAllPurchaseRequest()
                                .Where(p => (p.UserApprove1Id == getUserActiveId && p.ApproveStatusUser1 == null)
                                || (p.UserApprove2Id == getUserActiveId && p.ApproveStatusUser1 == "Approve" && p.ApproveStatusUser2 == null)
                                || (p.UserApprove3Id == getUserActiveId && p.ApproveStatusUser1 == "Approve" && p.ApproveStatusUser2 == "Approve" && p.ApproveStatusUser3 == null))
                                .OrderByDescending(a => a.CreateDateTime)
                                .ToList();

                var DataQtyDiff = _qtyDifferenceRepository.GetAllQtyDifference()
                                .Where(p => (p.UserApprove1Id == getUserActiveId && p.ApproveStatusUser1 == null)
                                || (p.UserApprove2Id == getUserActiveId && p.ApproveStatusUser1 == "Approve" && p.ApproveStatusUser2 == null))
                                .OrderByDescending(a => a.CreateDateTime)
                                .ToList();

                var DataUnitReq = _unitRequestRepository.GetAllUnitRequest()
                                .Where(p => (p.UserApprove1Id == getUserActiveId && p.ApproveStatusUser1 == null))
                                .OrderByDescending(a => a.CreateDateTime)
                                .ToList();


                foreach (var logger in DataPR)
                {
                    if (logger.ApproveStatusUser1 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalRepository.GetAllApproval().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User1" && u.PurchaseRequestNumber == logger.PurchaseRequestNumber).FirstOrDefault().ApprovalId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            purchaseRequestNumber = logger.PurchaseRequestNumber,
                            createdDate = logger.CreateDateTime
                        };
                        loggerDataPR.Add(detail);
                    }
                    else if (logger.ApproveStatusUser1 == "Approve" && logger.ApproveStatusUser2 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalRepository.GetAllApproval().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User2" && u.PurchaseRequestNumber == logger.PurchaseRequestNumber).FirstOrDefault().ApprovalId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            purchaseRequestNumber = logger.PurchaseRequestNumber,
                            createdDate = logger.CreateDateTime.ToString("dd/MM/yyyy HH:mm")
                        };
                        loggerDataPR.Add(detail);
                    }
                    else if (logger.ApproveStatusUser1 == "Approve" && logger.ApproveStatusUser2 == "Approve" && logger.ApproveStatusUser3 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalRepository.GetAllApproval().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User3" && u.PurchaseRequestNumber == logger.PurchaseRequestNumber).FirstOrDefault().ApprovalId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            purchaseRequestNumber = logger.PurchaseRequestNumber,
                            createdDate = logger.CreateDateTime
                        };
                        loggerDataPR.Add(detail);
                    }
                }

                foreach (var logger in DataQtyDiff)
                {
                    if (logger.ApproveStatusUser1 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalQtyDifferenceRepository.GetAllApproval().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User1" && u.QtyDifferenceId == logger.QtyDifferenceId).FirstOrDefault().ApprovalQtyDifferenceId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            qtyDifferenceNumber = logger.QtyDifferenceNumber,
                            createdDate = logger.CreateDateTime
                        };
                        loggerDataQtyDiff.Add(detail);
                    }
                    else if (logger.ApproveStatusUser1 == "Approve" && logger.ApproveStatusUser2 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalQtyDifferenceRepository.GetAllApproval().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User2" && u.QtyDifferenceId == logger.QtyDifferenceId).FirstOrDefault().ApprovalQtyDifferenceId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            qtyDifferenceNumber = logger.QtyDifferenceNumber,
                            createdDate = logger.CreateDateTime
                        };
                        loggerDataQtyDiff.Add(detail);
                    }
                }

                foreach (var logger in DataUnitReq)
                {
                    if (logger.ApproveStatusUser1 == null && logger.ApplicationUser.KodeUser == getUserActiveCode)
                    {
                        var getUserApproveId = _approvalUnitRequestRepository.GetAllApprovalRequest().Where(u => u.UserApproveId == getUserActiveId && u.ApprovalStatusUser == "User1" && u.UnitRequestId == logger.UnitRequestId).FirstOrDefault().ApprovalUnitRequestId;

                        var detail = new
                        {
                            approvalId = getUserApproveId,
                            createdBy = _userActiveRepository.GetAllUserLogin().Where(u => u.Id == logger.CreateBy.ToString()).FirstOrDefault()?.NamaUser,
                            unitRequestNumber = logger.UnitRequestNumber,
                            createdDate = logger.CreateDateTime
                        };
                        loggerDataUnitReq.Add(detail);
                    }
                }

                if (loggerDataPR.Count == 0 && loggerDataQtyDiff.Count == 0 && loggerDataUnitReq.Count == 0)
                {
                    var totalNotification = 0;
                    return Json(new { success = true, totalJsonAllNotification = totalNotification, loggerDataJsonPR = loggerDataPR, loggerDataJsonQtyDiff = loggerDataQtyDiff, loggerDataJsonUnitReq = loggerDataUnitReq });
                }
                else 
                {
                    var totalNotification = DataPR.Count + DataQtyDiff.Count + DataUnitReq.Count;
                    return Json(new { success = true, totalJsonAllNotification = totalNotification, loggerDataJsonPR = loggerDataPR, loggerDataJsonQtyDiff = loggerDataQtyDiff, loggerDataJsonUnitReq = loggerDataUnitReq });
                }                
            }                       
        }

        private string ProcessUploadFile(UserActiveViewModel model)
        {
            if(model.Foto == null)
            {
                return null;
            }

            string[] FileAkses = { ".jpg", ".jepg", ".png", ".gif" };
            string fileExtensions = Path.GetExtension(model.Foto.FileName).ToLowerInvariant();

            if (!FileAkses.Contains(fileExtensions))
            {
                throw new InvalidOperationException("format file harus png, jpg, jepg, gif");
            }

            if (model.Foto.Length > 2 * 1024 * 1024)
            {
                throw new InvalidOperationException("ukuran file telah melebihi batas maksimum 2MB");
            }

            string safeFileName = Path.GetFileNameWithoutExtension(model.Foto.FileName);
            safeFileName = Regex.Replace(safeFileName, @"[^a-zA-Z0-9-_]", "");

            var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}_{DateTime.Now.Ticks}{fileExtensions}";
            string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "UserPhoto");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Foto.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

    }
}
