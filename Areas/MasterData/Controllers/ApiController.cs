using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Data;
using System.Data;
using System.Net.Http;
using System.Security.Policy;

namespace PurchasingSystemDeveloper.Areas.MasterData.Controllers
{
    [Area("MasterData")]
    [Route("MasterData/[Controller]/[Action]")]
    public class ApiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMeasurementRepository _MeasurementRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ISupplierRepository _supplierRepository;

        public ApiController(
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            ICategoryRepository CategoryRepository,
            IMeasurementRepository MeasurementRepository,
            IDiscountRepository DiscountRepository,
            ISupplierRepository SupplierRepository,

            HttpClient httpClient
        )
        {
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _httpClient = httpClient;
            _categoryRepository = CategoryRepository;
            _productRepository = productRepository;
            _MeasurementRepository = MeasurementRepository;
            _discountRepository = DiscountRepository;
            _supplierRepository = SupplierRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData() { return View(); }
        public IActionResult GetDataSupplier() { return View(); }
        public IActionResult GetDataSatuan() { return View(); }
        public IActionResult GetDataKategoriObat() { return View(); }
        public IActionResult GetDataObat() { return View(); }
        public IActionResult GetDataDiskon() { return View(); }
        public IActionResult Impor() { return View(); }
        //[HttpGet]
        //public async Task<IActionResult> Impor(string apiCode)
        //{
        //    var apiUrl = apiCode; // URL API GetProduct

        //    // Mengirimkan permintaan GET ke API
        //    var response = await _httpClient.GetAsync(apiUrl);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var jsonData = await response.Content.ReadAsStringAsync();
        //        var productData = JsonConvert.DeserializeObject<List<Product>>(jsonData); // Deserialisasi JSON ke model Product

        //        return View("Impor", productData); // Tampilkan data dalam View
        //    }
        //    else
        //    {
        //        // Jika request gagal
        //        return View("Error", "Gagal mengambil data dari API");
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> CreateSupplier(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

            // Menambahkan header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E"); // Pastikan token atau kredensial benar

            var getUser = _userActiveRepository.GetAllUserLogin()
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();
            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");
            try
            {
                // Mengirimkan permintaan GET ke API
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                    var supplierData = responseObject.data.ToObject<List<dynamic>>();
                    var filterDisc = new List<dynamic>();

                    foreach (var item in supplierData)
                    {
                        //var getDiscVal = _supplierRepository.GetAllSupplier()
                        //    .FirstOrDefault(d => d.SupplierName == (string)Convert.ToDouble(item.nama_supp));

                        //if (getDiscVal == null)
                        //{
                            //filterDisc.Add(item);

                            // Mendapatkan kode measurement terakhir berdasarkan hari ini
                            var lastCode = _supplierRepository.GetAllSupplier()
                            .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                            .OrderByDescending(k => k.SupplierCode)
                            .FirstOrDefault();

                            string SupplierCode;

                            if (lastCode == null)
                            {
                                SupplierCode = "PCP" + setDateNow + "0001";
                            }
                            else
                            {
                                var lastCodeTrim = lastCode.SupplierCode.Substring(3, 6);

                                if (lastCodeTrim != setDateNow)
                                {
                                    SupplierCode = "PCP" + setDateNow + "0001";
                                }
                                else
                                {
                                    SupplierCode = "PCP" + setDateNow +
                                                    (Convert.ToInt32(lastCode.SupplierCode.Substring(9, lastCode.SupplierCode.Length - 9)) + 1)
                                                    .ToString("D4");
                                }
                            }

                            var supplier = new Supplier
                            {
                                CreateDateTime = DateTime.Now,
                                CreateBy = new Guid(getUser.Id),
                                SupplierId = Guid.NewGuid(),
                                SupplierCode = SupplierCode,
                                SupplierName = item.nama_supp,
                                LeadTimeId = new Guid("28D557A4-DFF5-45D8-7AF6-08DCAD5EBA2E"),// id leadtime 7
                                Address = item.alamat_supp,
                                Handphone = item.hp,
                                Email = "Supplier@email.com",
                                Note = item.ket,
                                IsPKS = true,
                                IsActive = true,
                            };
                            // Simpan ke database
                            _supplierRepository.Tambah(supplier);
                        //}
                    }

                    return View("CreateSupplier", supplierData); // Kirimkan data ke View
                }
                else
                {
                    // Jika request gagal
                    return View("Error", "Gagal mengambil data dari API");
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan jika ada masalah dengan koneksi atau pemrosesan data
                return View("Error", $"Terjadi kesalahan: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CreateSatuan(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            // Menambahkan header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E"); // Pastikan token atau kredensial benar

            try
            {
                // Mengirimkan permintaan GET ke API
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                    var createSatuanList = responseObject.data.ToObject<List<dynamic>>();

                    // Format tanggal sekarang (yyMMdd)
                    var setDateNow = DateTime.Now.ToString("yyMMdd");

                    // Convert dynamic data to List of Measurement models and save them
                    foreach (var item in createSatuanList)
                    {
                        // Mendapatkan kode measurement terakhir berdasarkan hari ini
                        var lastCode = _MeasurementRepository.GetAllMeasurement()
                            .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                            .OrderByDescending(k => k.MeasurementCode)
                            .FirstOrDefault();

                        string measurementCode;
                        if (lastCode == null)
                        {
                            measurementCode = "MSR" + setDateNow + "0001";
                        }
                        else
                        {
                            var lastCodeTrim = lastCode.MeasurementCode.Substring(3, 6);

                            if (lastCodeTrim != setDateNow)
                            {
                                measurementCode = "MSR" + setDateNow + "0001";
                            }
                            else
                            {
                                measurementCode = "MSR" + setDateNow + (Convert.ToInt32(lastCode.MeasurementCode.Substring(9, lastCode.MeasurementCode.Length - 9)) + 1).ToString("D4");
                            }
                        }

                        // Buat objek measurement baru
                        var measurement = new Measurement
                        {
                            CreateDateTime = DateTime.Now,
                            CreateBy = new Guid(getUser.Id),
                            MeasurementId = Guid.NewGuid(),
                            MeasurementName = item.satuan, // Pastikan item.satuan ada
                            MeasurementCode = measurementCode // Assign kode measurement yang baru
                        };

                        // Simpan ke database
                        _MeasurementRepository.Tambah(measurement);

                    }

                    return View("CreateSatuan", createSatuanList); // Kirimkan data ke View
                }
                else
                {
                    // Jika request gagal
                    return View("Error", "Gagal mengambil data dari API");
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan jika ada masalah dengan koneksi atau pemrosesan data
                return View("Error", $"Terjadi kesalahan: {ex.Message}");
            }
        }

        private async Task<T> GetApiDataAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            return default(T); // Mengembalikan nilai default jika gagal
        }

        [HttpGet]
        public async Task<IActionResult> CreateObat(string apiCode, int page = 1, int pageSize = 100)
        {
            var apiUrl = $"{apiCode}?page={page}&pageSize={pageSize}";

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E");

            var getUser = _userActiveRepository.GetAllUserLogin().FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (getUser == null)
            {
                return View("Error", "Pengguna tidak ditemukan.");
            }

                // Request untuk data obat
                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return View("Error", "Gagal mengambil data obat dari API.");
                }

                var jsonData = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var createObatList = responseObject.data.ToObject<List<dynamic>>();

                // Format tanggal sekarang (yyMMdd)
                var setDateNow = DateTime.Now.ToString("yyMMdd");

                foreach (var item in createObatList)
                {
                    //var apiDiscountUrl = $"https://app.mmchospital.co.id/devel_mantap/api.php?mod=api&cmd=get_diskon_obat&return_type=json&diskon_obat={item.kode_brg}";
                    //var responseDiscount = await _httpClient.GetAsync(apiDiscountUrl);
                    //var jsonDataDiscount = await responseDiscount.Content.ReadAsStringAsync();
                    //var responseObjectDiscount = JsonConvert.DeserializeObject<dynamic>(jsonDataDiscount);
                    //var createDiscount = responseObjectDiscount?.data.ToObject<List<dynamic>>();

                    var discountValue = "0";
                //var discountValue = $"{createDiscount.disc_persen}";
                if (discountValue == "")
                    {
                        discountValue = "0";
                    }

                    int discountIntValue = Convert.ToInt32(discountValue);

                    var supplierNameS = $"{item.supplier}";
                    if (supplierNameS == "")
                    {
                        supplierNameS = "PARIT PADANG GLOBAL, PT";
                    }
                    var categoryS = $"{item.commodity }";
                    if (categoryS =="")
                    {
                        categoryS = "OBAT";
                    }
                    var measurementS = $"{item.kode_satuan}";
                    if (measurementS == "")
                    {
                        measurementS = "ROL";
                    }
                    var getSupplier = _supplierRepository.GetAllSupplier().FirstOrDefault(u => u.SupplierName == supplierNameS);
                    var getCategory = _categoryRepository.GetAllCategory().FirstOrDefault(u => u.CategoryName == categoryS);
                    var getMeasurement = _MeasurementRepository.GetAllMeasurement().FirstOrDefault(u => u.MeasurementName == measurementS);
                    var getDiscount = _discountRepository.GetAllDiscount().FirstOrDefault(u => u.DiscountValue == discountIntValue);

                    // Mendapatkan kode product terakhir berdasarkan hari ini
                    var lastCode = _productRepository.GetAllProduct()
                        .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                        .OrderByDescending(k => k.ProductCode)
                        .FirstOrDefault();

                    string productCode;

                    if (lastCode == null)
                    {
                        productCode = "PDC" + setDateNow + "00001";
                    }
                    else
                    {
                        var lastCodeTrim = lastCode.ProductCode.Substring(3, 6);

                        if (lastCodeTrim != setDateNow)
                        {
                            productCode = "PDC" + setDateNow + "00001";
                        }
                        else
                        {
                            productCode = "PDC" + setDateNow +
                                            (Convert.ToInt32(lastCode.ProductCode.Substring(9, lastCode.ProductCode.Length - 9)) + 1)
                                            .ToString("D4");
                        }
                    }

                    // Membangun objek Product untuk disimpan
                    var product = new Product
                    {
                        CreateDateTime = DateTime.Now,
                        CreateBy = new Guid(getUser.Id),
                        ProductId = Guid.NewGuid(),
                        ProductCode = productCode,
                        ProductName = item.nama_brg, 
                        SupplierId = getSupplier?.SupplierId ?? Guid.Empty,
                        CategoryId = getCategory?.CategoryId ?? new Guid("5B26524F-B843-4E63-4A96-08DCACC980CC"),
                        MeasurementId = getMeasurement?.MeasurementId ?? Guid.Empty,
                        DiscountId = getDiscount?.DiscountId ?? Guid.Empty,
                        WarehouseLocationId = new Guid("4218A796-79B1-4F59-7767-08DCAE28EBBE"),
                        MinStock = 0,
                        MaxStock = 0,
                        BufferStock = 0,
                        Stock = 0,
                        Cogs = 0m, 
                        BuyPrice = 0m,
                        RetailPrice = 0m,
                        StorageLocation = "BOX",
                        RackNumber = "4353",
                        Note = "HSIS"
                    };

                    // Simpan ke database
                    _productRepository.Tambah(product);
                }

                return View("CreateObat", createObatList); // Mengirimkan data ke view
           
        }

        [HttpGet]
        public async Task<IActionResult> CreateKategoriObat(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

            var getUser = _userActiveRepository.GetAllUserLogin().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            // Menambahkan header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E"); // Pastikan token atau kredensial benar

            try
            {
                // Mengirimkan permintaan GET ke API
                var response = await _httpClient.GetAsync(apiUrl);

                var dateNow = DateTimeOffset.Now;
                var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();

                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                    var CreateKategoriObat = responseObject.data.ToObject<List<dynamic>>();
                    var createKategoriList = responseObject.data.ToObject<List<dynamic>>();

                    foreach (var item in createKategoriList)
                    {
                        // Mendapatkan kode measurement terakhir berdasarkan hari ini
                        var lastCode = _categoryRepository.GetAllCategory()
                            .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                            .OrderByDescending(k => k.CategoryCode)
                            .FirstOrDefault();


                        string kategoriCode;
                        if (lastCode == null)
                        {
                            kategoriCode = "CTG" + setDateNow + "0001";
                        }
                        else
                        {
                            var lastCodeTrim = lastCode.CategoryCode.Substring(3, 6);

                            if (lastCodeTrim != setDateNow)
                            {
                                kategoriCode = "CTG" + setDateNow + "0001";
                            }
                            else
                            {
                                kategoriCode = "CTG" + setDateNow + (Convert.ToInt32(lastCode.CategoryCode.Substring(9, lastCode.CategoryCode.Length - 9)) + 1).ToString("D4");
                            }
                        }
                        // Buat objek measurement baru
                        var Category = new Category
                        {
                            CreateDateTime = DateTime.Now,
                            CreateBy = new Guid(getUser.Id),
                            CategoryId = Guid.NewGuid(),
                            CategoryCode = kategoriCode,
                            CategoryName = item.kel_brg,
                        };

                        // Simpan ke database
                        _categoryRepository.Tambah(Category);

                    }
                    return View("CreateKategoriObat", CreateKategoriObat); // Kirimkan data ke View
                }
                else
                {
                    // Jika request gagal
                    return View("Error", "Gagal mengambil data dari API");
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan jika ada masalah dengan koneksi atau pemrosesan data
                return View("Error", $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateDiskon(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

            var getUser = _userActiveRepository.GetAllUserLogin()
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();

            // Menambahkan header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E"); // Pastikan token atau kredensial benar

            // Mengirimkan permintaan GET ke API
            var response = await _httpClient.GetAsync(apiUrl);

            var dateNow = DateTimeOffset.Now;
            var setDateNow = DateTimeOffset.Now.ToString("yyMMdd");

            if (response.IsSuccessStatusCode)
            {
                // Membaca konten API
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var diskoniList = responseObject.data.ToObject<List<dynamic>>();
                var filterDisc = new List<dynamic>();

                foreach (var item in diskoniList)
                {
                    var getDiscVal = _discountRepository.GetAllDiscount().Where(d => d.DiscountValue == (int)Convert.ToDouble(item.disc_persen)).FirstOrDefault();

                    if (getDiscVal == null)
                    {
                        filterDisc.Add(item);

                        // Mendapatkan kode measurement terakhir berdasarkan hari ini
                        var lastCode = _discountRepository.GetAllDiscount()
                        .Where(d => d.CreateDateTime.ToString("yyMMdd") == setDateNow)
                        .OrderByDescending(k => k.DiscountCode)
                        .FirstOrDefault();

                        string DiscountCode;

                        if (lastCode == null)
                        {
                            DiscountCode = "DSC" + setDateNow + "0001";
                        }
                        else
                        {
                            var lastCodeTrim = lastCode.DiscountCode.Substring(3, 6);

                            if (lastCodeTrim != setDateNow)
                            {
                                DiscountCode = "DSC" + setDateNow + "0001";
                            }
                            else
                            {
                                DiscountCode = "DSC" + setDateNow +
                                                (Convert.ToInt32(lastCode.DiscountCode.Substring(9, lastCode.DiscountCode.Length - 9)) + 1)
                                                .ToString("D4");
                            }
                        }

                        // Membuat objek diskon baru// Cek apakah nilai disc_persen tidak null
                        var discountValue = item.disc_persen != null
                            ? (int)Convert.ToDouble(item.disc_persen) // Membuang angka desimal tanpa pembulatan
                            : 0;

                        var discount = new Discount
                        {
                            CreateDateTime = DateTimeOffset.Now,
                            CreateBy = new Guid(getUser.Id),
                            DiscountId = Guid.NewGuid(),
                            DiscountCode = DiscountCode,
                            DiscountValue = discountValue // Menangani null atau jika data tidak ada
                        };

                        // Simpan ke database
                        _discountRepository.Tambah(discount);
                    }
                }

                // Kirimkan data ke View
                return View("CreateDiskon", filterDisc); // Kirimkan hanya 100 data
            }
            else
            {
                // Jika request gagal, tampilkan pesan error
                return View("Error", "Gagal mengambil data dari API");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct(List<Product> products)
        {
            var apiUrl = "http://192.168.15.250:7311/MasterData/Product/GetProduct";

            // Mengirimkan permintaan GET ke API
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Membaca data dari API
                var jsonData = await response.Content.ReadAsStringAsync();


                // Deserialisasi data JSON ke List<Product>
                var productData = JsonConvert.DeserializeObject<List<Product>>(jsonData);

                // Menyimpan produk ke dalam database
                foreach (var product in productData)
                {
                    // Simpan produk satu per satu ke database
                    //product.SupplierId = product.PrincipalId;
                    _productRepository.Tambah(product);
                }
                // Memeriksa apakah model valid
                // Setelah menyimpan, arahkan ke halaman sukses atau index
                return RedirectToAction("Index", "Api");
            }

            // Jika ada kesalahan, kembali ke view dengan data yang ada
            // Anda mungkin ingin mengisi ViewBag untuk memberikan pesan error kepada pengguna
            ViewBag.ErrorMessage = "Terjadi kesalahan saat menyimpan produk. Silakan periksa input Anda.";
            return View("Impor", products);
        }

    }
}