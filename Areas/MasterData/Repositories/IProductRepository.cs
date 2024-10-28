using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;
using System.Diagnostics.Metrics;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
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
                    CategoryId = Product.CategoryId,
                    MeasurementId = Product.MeasurementId,
                    DiscountId = Product.DiscountId,
                    WarehouseLocationId = Product.WarehouseLocationId,
                    MinStock = Product.MinStock,
                    MaxStock = Product.MaxStock,
                    BufferStock = Product.BufferStock,
                    Stock = Product.Stock,
                    Cogs = Product.Cogs,
                    BuyPrice = Product.BuyPrice,
                    RetailPrice = Product.RetailPrice,
                    StorageLocation = Product.StorageLocation,
                    RackNumber = Product.RackNumber,
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
            return await _context.Products.OrderBy(p => p.CreateDateTime).Select(Product => new Product()
            {
                ProductId = Product.ProductId,
                ProductCode = Product.ProductCode,
                ProductName = Product.ProductName,
                Supplier = Product.Supplier,
                SupplierId = Product.SupplierId,
                CategoryId = Product.CategoryId,
                MeasurementId = Product.MeasurementId,
                DiscountId = Product.DiscountId,
                WarehouseLocationId = Product.WarehouseLocationId,
                MinStock = Product.MinStock,
                MaxStock = Product.MaxStock,
                BufferStock = Product.BufferStock,
                Stock = Product.Stock,
                Cogs = Product.Cogs,
                BuyPrice = Product.BuyPrice,
                RetailPrice = Product.RetailPrice,
                StorageLocation = Product.StorageLocation,
                RackNumber = Product.RackNumber,
                Note = Product.Note
            }).ToListAsync();
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return _context.Products.OrderByDescending(d => d.CreateDateTime)
                .Include(p => p.Supplier)
                .Include(c => c.Category)
                .Include(m => m.Measurement)
                .Include(d => d.Discount)
                .Include(w => w.WarehouseLocation)
                .AsNoTracking();
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
