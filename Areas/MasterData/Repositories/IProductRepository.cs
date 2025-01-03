using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;
using System.Diagnostics.Metrics;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    //public interface IProductRepository
    //{
    //    Task<(List<Product> Items, int TotalCount)> GetPagedProducts(string searchTerm, int page, int pageSize);
    //    // Method lain...
    //}
    public class IProductRepository 
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Product Tambah(Product Product)
        {
            _context.Products.Add(Product);
            _context.SaveChanges();
            return Product;
        }

        public async Task<Product> GetProductById(Guid Id)
        {
            var Product = await _context.Products
                .Where(a => a.IsActive == true)
                .Include(p => p.Supplier)
                .Include(c => c.Category)
                .Include(m => m.Measurement)
                .Include(d => d.Discount)
                .Include(w => w.WarehouseLocation)
                .SingleOrDefaultAsync(i => i.ProductId == Id);

            if (Product != null)
            {
                var ProductDetail = new Product()
                {
                    ProductId = Product.ProductId,
                    ProductCode = Product.ProductCode,
                    ProductName = Product.ProductName,
                    SupplierId = Product.SupplierId,
                    Supplier = Product.Supplier,
                    CategoryId = Product.CategoryId,
                    Category = Product.Category,
                    MeasurementId = Product.MeasurementId,
                    Measurement = Product.Measurement,
                    DiscountId = Product.DiscountId,
                    Discount = Product.Discount,
                    WarehouseLocationId = Product.WarehouseLocationId,
                    WarehouseLocation = Product.WarehouseLocation,
                    ExpiredDate = Product.ExpiredDate,
                    MinStock = Product.MinStock,
                    MaxStock = Product.MaxStock,
                    BufferStock = Product.BufferStock,
                    Stock = Product.Stock,
                    Cogs = Product.Cogs,
                    BuyPrice = Product.BuyPrice,
                    RetailPrice = Product.RetailPrice,
                    StorageLocation = Product.StorageLocation,
                    RackNumber = Product.RackNumber,
                    IsActive = Product.IsActive,
                    Note = Product.Note
                };
                return ProductDetail;
            }
            return null;
        }

        public async Task<Product> GetProductByIdNoTracking(Guid Id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(a => a.ProductId == Id);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products
                .Include(s => s.Supplier)
                .OrderBy(p => p.CreateDateTime)
                .Where(a => a.IsActive == true)
                //.Take(20)
                .Select(Product => new Product() {
                    ProductId = Product.ProductId,
                    ProductCode = Product.ProductCode,
                    ProductName = Product.ProductName,
                    SupplierId = Product.SupplierId,
                    Supplier = Product.Supplier,
                    CategoryId = Product.CategoryId,
                    Category = Product.Category,
                    MeasurementId = Product.MeasurementId,
                    Measurement = Product.Measurement,
                    DiscountId = Product.DiscountId,
                    Discount = Product.Discount,
                    WarehouseLocationId = Product.WarehouseLocationId,
                    WarehouseLocation = Product.WarehouseLocation,
                    ExpiredDate = Product.ExpiredDate,
                    MinStock = Product.MinStock,
                    MaxStock = Product.MaxStock,
                    BufferStock = Product.BufferStock,
                    Stock = Product.Stock,
                    Cogs = Product.Cogs,
                    BuyPrice = Product.BuyPrice,
                    RetailPrice = Product.RetailPrice,
                    StorageLocation = Product.StorageLocation,
                    RackNumber = Product.RackNumber,
                    IsActive = Product.IsActive,
                    Note = Product.Note
                }).ToListAsync();
        }        

        public IEnumerable<Product> GetAllProduct()
        {
            return _context.Products.OrderByDescending(d => d.CreateDateTime).Where(a => a.IsActive == true)
                .Include(p => p.Supplier)
                .Include(c => c.Category)
                .Include(m => m.Measurement)
                .Include(d => d.Discount)
                .Include(w => w.WarehouseLocation)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<Product> products, int totalCountProducts)> GetAllProductPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Products
                .OrderByDescending(d => d.CreateDateTime)
                .Where(a => a.IsActive == true)
                .Include(p => p.Supplier)
                .Include(c => c.Category)
                .Include(m => m.Measurement)
                .Include(d => d.Discount)
                .Include(w => w.WarehouseLocation)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductCode.Contains(searchTerm) || p.ProductName.Contains(searchTerm) || p.Supplier.SupplierName.Contains(searchTerm));
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.CreateDateTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.CreateDateTime <= endDate.Value);
            }

            var totalCount = await query.CountAsync();

            // Ambil data paginated
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public Product Update(Product update)
        {
            var Product = _context.Products.Attach(update);
            Product.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Product Delete(Guid Id)
        {
            var Product = _context.Products.Find(Id);
            if (Product != null)
            {
                _context.Products.Remove(Product);
                _context.SaveChanges();
            }
            return Product;
        }
    }
}
