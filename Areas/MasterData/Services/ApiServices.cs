using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Areas.MasterData.Repositories;
using PurchasingSystemDeveloper.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurchasingSystemDeveloper.Areas.MasterData.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiServices : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserActiveRepository _userActiveRepository;
        private readonly string _hardcodedJwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        public ApiServices(
            IProductRepository productRepository,
            ApplicationDbContext applicationDbContext,
            IUserActiveRepository userActiveRepository)
        {
            _applicationDbContext = applicationDbContext;
            _userActiveRepository = userActiveRepository;
            _productRepository = productRepository;
        }

        // Fungsi untuk mendapatkan semua produk tanpa login
        [HttpGet("Product")] // Menggunakan path tambahan 'products'
        [AllowAnonymous] // Membolehkan akses tanpa login

        // https://localhost:7574/api/ApiServices/ThisIsASecretKey12345ForJWT 
        public async Task<ActionResult<List<Product>>> GetAllProductsAsync()
        {
            var products =  _productRepository.GetAllProduct(); // Get products from the repository

            // Memeriksa apakah produk ada
            if (products == null )
            {
                return NotFound("Tidak ada produk yang ditemukan.");
            }

            // Mengembalikan data produk dari database dalam bentuk response OK
            return Ok(products);
        }

        [HttpPost("login")]
        [AllowAnonymous] // Membolehkan akses tanpa login
        public IActionResult Login()
        {
            // Hardcode JWT untuk pengujian
            return Ok(new { token = _hardcodedJwt });
        }
    }
}
