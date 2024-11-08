using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Data;
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

        public ApiController(
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository,
            IProductRepository productRepository,
            HttpClient httpClient
        )
        {
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _httpClient = httpClient;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData(){return View(); }
        public IActionResult GetDataSupplier() { return View(); }
        public IActionResult GetDataSatuan() { return View(); }
        public IActionResult GetDataKategoriObat() { return View(); }
        public IActionResult GetDataObat() { return View(); }
        public IActionResult GetDataDiskon() { return View(); }

        [HttpGet]
        public async Task<IActionResult> Impor(string apiCode)
        {
            var apiUrl = apiCode; // URL API GetProduct

            // Mengirimkan permintaan GET ke API
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var productData = JsonConvert.DeserializeObject<List<Product>>(jsonData); // Deserialisasi JSON ke model Product

                return View("Impor", productData); // Tampilkan data dalam View
            }
            else
            {
                // Jika request gagal
                return View("Error", "Gagal mengambil data dari API");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateSupplier(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

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
                    var supplierData = responseObject.data.ToObject<List<dynamic>>();

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
                    var CreateSatuan = responseObject.data.ToObject<List<dynamic>>();

                    return View("CreateSatuan", CreateSatuan); // Kirimkan data ke View
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
        public async Task<IActionResult> CreateObat(string apiCode, int page = 1, int pageSize = 100)
        {
            var apiUrl = $"{apiCode}?page={page}&pageSize={pageSize}"; // Ubah API untuk mendukung pagination

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YWRtZWRpa2E6YWRtZWRpa2E");

            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                    var CreateObat = responseObject.data.ToObject<List<dynamic>>();
                    return View("CreateObat", CreateObat);
                }
                else
                {
                    return View("Error", "Gagal mengambil data dari API");
                }
            }
            catch (Exception ex)
            {
                return View("Error", $"Terjadi kesalahan: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateKategoriObat(string apiCode)
        {
            var apiUrl = apiCode; // URL API untuk mengambil data supplier

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
                    var CreateKategoriObat = responseObject.data.ToObject<List<dynamic>>();

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
                    var CreateDiskon = responseObject.data.ToObject<List<dynamic>>();

                    return View("CreateDiskon", CreateDiskon); // Kirimkan data ke View
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
